using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn;
using Yarn.Unity;

public class DialogueView : DialogueViewBase
{
    // The canvas group that contains the UI elements used by this Line View.
    [SerializeField]
    internal CanvasGroup canvasGroup;


    // Controls whether the line view should fade in when lines appear, and fade out when lines disappear.
 
    [SerializeField]
    internal bool useFadeEffect = true;
    
    // The time that the fade effect will take to fade lines in.
    [SerializeField]
    [Min(0)]
    internal float fadeInTime = 0.25f;
    
    // The time that the fade effect will take to fade lines out.
    
    [SerializeField]
    [Min(0)]
    internal float fadeOutTime = 0.05f;
    
    [HideInInspector] 
    private float currentFadeOutTime;

    // The <see cref="TextMeshProUGUI"/> object that displays the text of dialogue lines.
    [SerializeField]
    internal TextMeshProUGUI lineText = null;
    
    // Controls whether the <see cref="lineText"/> object will show the character name present in the line or not.
   
    [SerializeField]
    [UnityEngine.Serialization.FormerlySerializedAs("showCharacterName")]
    internal bool showCharacterNameInLineView = true;
    
    // The <see cref="TextMeshProUGUI"/> object that displays the character names found in dialogue lines.

    [SerializeField]
    internal TextMeshProUGUI characterNameText = null;
    
    // The gameobject that holds the <see cref="characterNameText"/> textfield.
    [SerializeField] 
    internal GameObject characterNameContainer = null;
    
    // Controls whether the text of <see cref="lineText"/> should be gradually revealed over time.
    [SerializeField]
    internal bool useTypewriterEffect = false;

    // A Unity Event that is called each time a character is revealed during a typewriter effect.
    [SerializeField]
    internal UnityEngine.Events.UnityEvent onCharacterTyped;
    
    // A Unity Event that is called when a pause inside of the typewriter effect occurs.
    [SerializeField] 
    internal UnityEngine.Events.UnityEvent onPauseStarted;
    
    // A Unity Event that is called when a pause inside of the typewriter effect finishes and the typewriter has started once again.
    [SerializeField] 
    internal UnityEngine.Events.UnityEvent onPauseEnded;
    
    // The number of characters per second that should appear during a typewriter effect.
    [SerializeField]
    [Min(0)]
    internal float typewriterEffectSpeed = 0f;
    
    // Continue Button image so we can display it but not have a function
    [SerializeField]
    internal GameObject continueButtonImage;

    // The amount of time to wait after any line
    [SerializeField]
    [Min(0)]
    internal float holdTime = 1f;
    
    // Controls whether this Line View will wait for user input before indicating that it has finished presenting a line.
    [SerializeField]
    internal bool autoAdvance = false;
    
    // The current <see cref="LocalizedLine"/> that this line view is displaying.
    LocalizedLine currentLine = null;
    
    // A stop token that is used to interrupt the current animation.
    Effects.CoroutineInterruptToken currentStopToken = new Effects.CoroutineInterruptToken();

    // skip lastline display and go straight to options
    [SerializeField] 
    YarnProject yarnProject;
    string[] isLastLine = new string[1];
    
    private void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
    
    private void Reset()
    {
        canvasGroup = GetComponentInParent<CanvasGroup>();
    }
    
    public override void DismissLine(Action onDismissalComplete)
    {
        currentLine = null;
        StartCoroutine(DismissLineInternal(onDismissalComplete));
    }

    private IEnumerator DismissLineInternal(Action onDismissalComplete)
    {
        // disabling interaction temporarily while dismissing the line
        // we don't want people to interrupt a dismissal
        var interactable = canvasGroup.interactable;
        canvasGroup.interactable = false;

        // If we're using a fade effect, run it, and wait for it to finish.
        if (useFadeEffect)
        {
            // if it's the lastline before options use no fadeout
            if(isLastLine != null)
            {
                currentFadeOutTime = 0f;
            }
            else
            {
                currentFadeOutTime = fadeOutTime;
            }

            yield return StartCoroutine(Effects.FadeAlpha(canvasGroup, 1, 0, currentFadeOutTime, currentStopToken));
            currentStopToken.Complete();
        }

        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        
        // turning interaction back on, if it needs it
        canvasGroup.interactable = interactable;

        if (onDismissalComplete != null)
        {
            onDismissalComplete();
        }
    }
    
    public override void InterruptLine(LocalizedLine dialogueLine, Action onInterruptLineFinished)
    {
        currentLine = dialogueLine;

        // Cancel all coroutines that we're currently running. This will stop the RunLineInternal coroutine, if it's running.
        StopAllCoroutines();

        // for now we are going to just immediately show everything later we will make it fade in
        lineText.gameObject.SetActive(true);
        canvasGroup.gameObject.SetActive(true);

        int length;

        if (characterNameText == null)
        {
            if (showCharacterNameInLineView)
            {
                lineText.text = dialogueLine.Text.Text;
                length = dialogueLine.Text.Text.Length;
            }
            else
            {
                lineText.text = dialogueLine.TextWithoutCharacterName.Text;
                length = dialogueLine.TextWithoutCharacterName.Text.Length;
            }
        }
        else
        {
            characterNameText.text = dialogueLine.CharacterName;
            lineText.text = dialogueLine.TextWithoutCharacterName.Text;
            length = dialogueLine.TextWithoutCharacterName.Text.Length;
        }

        // Show the entire line's text immediately.
        lineText.maxVisibleCharacters = length;

        // Make the canvas group fully visible immediately, too.
        canvasGroup.alpha = 1;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        onInterruptLineFinished();
    }
    
    public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        // Stop any coroutines currently running on this line view (for example, any other RunLine that might be running)
        StopAllCoroutines();

        // Begin running the line as a coroutine.
        StartCoroutine(RunLineInternal(dialogueLine, onDialogueLineFinished));
    }

    private IEnumerator RunLineInternal(LocalizedLine dialogueLine, Action onDialogueLineFinished)
    {
        isLastLine = yarnProject.lineMetadata.GetMetadata(dialogueLine.TextID);

        IEnumerator PresentLine()
        {
            // If its the lastline before a option we skip straight to optionview 
            if (isLastLine != null)
            {
                UserRequestedViewAdvancement();
                yield break;
            }

            // If not then display linetext as normal
            lineText.gameObject.SetActive(true);
            canvasGroup.gameObject.SetActive(true);
            
            // Hide the continue button until presentation is complete (if we have one).
            if (continueButtonImage != null)
            {
                continueButtonImage.SetActive(false);
            }
            
            Yarn.Markup.MarkupParseResult text = dialogueLine.TextWithoutCharacterName;
            
            if (characterNameContainer != null && characterNameText != null)
            {
                // we are set up to show a character name, but there isn't one so just hide the container
                if (string.IsNullOrWhiteSpace(dialogueLine.CharacterName))
                {
                    characterNameContainer.SetActive(false);
                }
                else
                {
                    // we have a character name text view, show the character name
                    characterNameText.text = dialogueLine.CharacterName;
                    characterNameContainer.SetActive(true);
                }
            }
            else
            {
                // We don't have a character name text view. Should we show the character name in the main text view?
                if (showCharacterNameInLineView)
                {
                    // Yep! Show the entire text.
                    text = dialogueLine.Text;
                }
            }
            
            lineText.text = text.Text;
             
            if (useTypewriterEffect)
            {
                /*
                 * If we're using the typewriter effect, hide all of the
                 * text before we begin any possible fade (so we don't fade
                 * in on visible text).
                 */
                lineText.maxVisibleCharacters = 0;
            }
            else
            {
                // Ensure that the max visible characters is effectively unlimited.
                lineText.maxVisibleCharacters = int.MaxValue;
            }

            // If we're using the fade effect, start it, and wait for it to finish.
            if (useFadeEffect)
            {
                yield return StartCoroutine(Effects.FadeAlpha(canvasGroup, 0, 1, fadeInTime, currentStopToken));
                
                if (currentStopToken.WasInterrupted)
                {
                    // The fade effect was interrupted. Stop this entire coroutine.
                    yield break;
                }
            }

            // If we're using the typewriter effect, start it, and wait for it to finish.
            if (useTypewriterEffect)
            {
                var pauses = LineView.GetPauseDurationsInsideLine(text);

                // setting the canvas all back to its defaults because if we didn't also fade we don't have anything visible
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                yield return StartCoroutine(Effects.PausableTypewriter(
                    lineText,
                    typewriterEffectSpeed,
                    () => onCharacterTyped.Invoke(),
                    () => onPauseStarted.Invoke(),
                    () => onPauseEnded.Invoke(),
                    pauses,
                    currentStopToken
                ));

                if (currentStopToken.WasInterrupted)
                {
                    // The typewriter effect was interrupted. Stop this entire coroutine.
                    yield break;
                }
            }
        }
        
        currentLine = dialogueLine;
        
        /*
         * Run any presentations as a single coroutine. If this is stopped,
         * which UserRequestedViewAdvancement can do, then we will stop all
         * of the animations at once.
         */
        yield return StartCoroutine(PresentLine());

        currentStopToken.Complete();

        // All of our text should now be visible.
        lineText.maxVisibleCharacters = int.MaxValue;

        // Our view should at be at full opacity.
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Show the continue button, if we have one.
        if (continueButtonImage != null)
        {
            continueButtonImage.SetActive(true);
        }

        // If we have a hold time, wait that amount of time, and then continue.
        if (holdTime > 0)
        {
            yield return new WaitForSeconds(holdTime);
        }

        if (autoAdvance == false)
        {
            /*
             * The line is now fully visible, and we've been asked to not
             * auto-advance to the next line. Stop here, and don't call the
             * completion handler - we'll wait for a call to
             * UserRequestedViewAdvancement, which will interrupt this
             * UserRequestedViewAdvancement, which will interrupt this coroutine.
             */
            yield break;
        }

        // Our presentation is complete; call the completion handler.
        onDialogueLineFinished();
    }
    
    /// <inheritdoc/>
    public override void UserRequestedViewAdvancement()
    {
        /*
         * We received a request to advance the view. If we're in the middle of
         * an animation, skip to the end of it. If we're not current in an
         * animation, interrupt the line so we can skip to the next one.
         */
        
        // we have no line, so the user just mashed randomly
        if (currentLine == null)
        {
            return;
        }
        
        /*
         * We may want to change this later so the interrupted
         * animation coroutine is what actually interrupts.
         * for now this is fine.
         * Is an animation running that we can stop?
         */
        
        if (currentStopToken.CanInterrupt)
        {
            // Stop the current animation, and skip to the end of whatever started it.
            currentStopToken.Interrupt();
        }
        else
        {
            // No animation is now running. Signal that we want to interrupt the line instead.
            requestInterrupt?.Invoke();
        }
    }

    /// <summary>
    /// Called when the <see cref="continueButton"/> is clicked.
    /// </summary>
    public void OnContinueClicked()
    {
        /*
         * When the Continue button is clicked, we'll do the same thing as
         * if we'd received a signal from any other part of the game
         * (for example, if a DialogueAdvanceInput had signalled us)
         */
        UserRequestedViewAdvancement();
    }
}