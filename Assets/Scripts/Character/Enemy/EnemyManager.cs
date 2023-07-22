using UnityEngine;

namespace Character.Enemy
{
    public class EnemyManager : CharacterManager
    {
        private const float DetectionRadius = 10f;
        private const float MovementSpeed = 3.5f;
        private const float RotationSpeed = 3.5f;
        private const float StoppingDistance = .5f;
        private const float AttackDistance = .5f;
        private const float AnimationSpeedMultiplier = 2f;
        private const float Acceleration = 5f;
        private const float Deceleration = 5f;

        private Transform playerTransform;

        private float currentSpeed;
        private float targetSpeed;

        protected override void Awake()
        {
            base.Awake();
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            AssignAnimationIDs();
        }

        protected override void Update()
        {
            base.Update();
            MoveEnemy();
        }

        private void MoveEnemy()
        {
            Animator.SetFloat(AnimIDMotionSpeed, 1);
            Vector3 movement = Vector3.zero;
            isUsingRootMotion = Animator.GetBool(AnimIDIsUsingRootMotion);

            if (Grounded)
            {
                Animator.SetBool(AnimIDFreeFall, false);

                float distance = Vector3.Distance(transform.position, playerTransform.position);

                if (distance <= DetectionRadius)
                {
                    // Set the direction from the enemy to the player
                    Vector3 direction = (playerTransform.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);

                    // Calculate the target speed based on distance
                    targetSpeed = distance <= StoppingDistance ? 0f : MovementSpeed;

                    // Gradually adjust the current speed based on the target speed
                    if (currentSpeed < targetSpeed)
                        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * Acceleration);
                    else if (currentSpeed > targetSpeed)
                        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * Deceleration);

                    // Move the enemy towards the player
                    movement = transform.forward * (currentSpeed * Time.deltaTime);


                    // Update the animator's Speed parameter and animation speed
                    Animator.SetFloat(AnimIDSpeed, currentSpeed * AnimationSpeedMultiplier);
                }
                else
                {
                    // Stop the enemy
                    currentSpeed = 0f;
                    Animator.SetFloat(AnimIDSpeed, currentSpeed);
                }

                if (distance <= AttackDistance)
                {
                    if (!Animator.GetBool(AnimIDIsUsingRootMotion))
                    {
                        Animator.CrossFade("Attack_1", .2f);
                        Animator.SetBool(AnimIDIsUsingRootMotion, true);
                    }
                }
            }
            else
            {
                Falling();
            }

            // actually move the character
            if (!isUsingRootMotion)
            {
                characterController.Move(movement +
                                         new Vector3(0.0f, VerticalVelocity, 0.0f) * Time.deltaTime);
            }
        }

        private void Falling()
        {
            // fall timeout
            if (FallTimeoutDelta >= 0.0f)
            {
                FallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                Animator.SetBool(AnimIDFreeFall, true);
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (VerticalVelocity < TerminalVelocity)
            {
                VerticalVelocity += Gravity * Time.deltaTime;
            }

            var fallingSpeed = new Vector3(0, VerticalVelocity, 0) * Time.deltaTime;

            characterController.Move(fallingSpeed);
        }

        protected override void OnAnimatorMove()
        {
            base.OnAnimatorMove();
        }
    }
}