using UnityEngine;

public class BoredBehavior : StateMachineBehaviour
{
    [SerializeField] private float _timeUntilBored;
    [SerializeField] private int _numberOfBoredAnimations;

    // Keep track of whether the character is currently bored
    private bool _isBored;

    // Keep track of how long the character has been idle
    private float _idleTime;

    // The animation we want to transition to
    private int _boredAnimation;

    // The value of the animation parameter
    private static readonly int BoredAnimation = Animator.StringToHash("BoredAnimation");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset everything when we enter the idle state
        ResetIdle();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Check that we aren't currently bored
        if (_isBored == false)
        {
            // How much time is remaining in the current animation
            // "0" indicates the start of the animation and "1" indicates the end
            // But as the animation continues to loop, "1" would indicate the start and "2" would indicate the end
            // So on and so forth. To find if we are towards the end of a loop, we do modulus "1".
            // var remainingAnimationTime = stateInfo.normalizedTime % 1;
            _idleTime += Time.deltaTime;

            // Check if character has been idle long enough to become bored
            // Only change if we are at the beginning of an animation
            if (_idleTime > _timeUntilBored && stateInfo.normalizedTime % 1 < 0.02f)
            {
                _isBored = true;

                // Get the right random bored animation
                _boredAnimation = Random.Range(1, _numberOfBoredAnimations + 1);
                _boredAnimation = _boredAnimation * 2 - 1;

                // Set the animation parameter to the closet "Default Idle" animation
                animator.SetFloat(BoredAnimation, _boredAnimation - 1);
            }
        }
        // Nearing the end of one of the animation loops, reset back to the default idle animation
        else if (stateInfo.normalizedTime % 1 > 0.98)
        {
            ResetIdle();
        }

        // Trigger bored animation and use damping to transition gradually frame by frame
        animator.SetFloat(BoredAnimation, _boredAnimation, 0.2f, Time.deltaTime);
    }

    /// <summary>
    /// Reset the current animation back to the Idle state.
    /// </summary>
    private void ResetIdle()
    {
        if (_isBored)
        {
            // Go back to the previous default idle animation
            _boredAnimation--;
        }
        
        _isBored = false;
        _idleTime = 0;
    }
}