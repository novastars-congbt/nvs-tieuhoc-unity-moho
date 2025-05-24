using UnityEngine;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// Provides methods for random operations used in the hangman minigame.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns the result of a fifty fifty chance operation (true for greater than 0.5 and false for lower than 0.5)
        /// </summary>
        /// <returns>The result of the fifty fifty.</returns>
        public static bool FiftyFifty() => Random.Range(0, 1f) > 0.5f;

        /// <summary>
        /// Returns a vector3 with given offset from this vector3.
        /// </summary>
        /// <param name="vector3">The initial vector3 to offset from.</param>
        /// <param name="axis">The axis on which the offset is applied.</param>
        /// <param name="offset">The offset value.</param>
        /// <returns>The randomly offsetted value.</returns>
        public static Vector3 RandomOffset(this Vector3 vector3, Vector3 axis, float offset)
        {
            for(int i = 0; i < 3; i++)
                if (axis[i] == 1.0f)
                    vector3[i] += Random.Range(-offset, offset);

            return vector3;
        }
    }
}
