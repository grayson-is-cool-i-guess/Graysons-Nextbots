using System;
using System.Collections;
using UnityEngine;

namespace GraysonsNextbots.Nextbots
{
    public class NextbotManager : MonoBehaviour
    {
        private static float nextSpawnTime;

        public static void Summon()
        {
            if (Time.time > nextSpawnTime && ControllerInputPoller.instance.rightGrab)
            {
                GameObject goober = GameObject.CreatePrimitive(PrimitiveType.Quad);
                goober.name = "Nextbot";
                goober.transform.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position;
                goober.layer = 8;

                var rb = goober.AddComponent<Rigidbody>();
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                goober.AddComponent<BoxCollider>();

                Material mat = new Material(Shader.Find("Unlit/Texture"));
                mat.mainTexture = Plugin.nextbotLogo;
                goober.GetComponent<Renderer>().material = mat;

                AudioSource audio = goober.AddComponent<AudioSource>();
                audio.spatialBlend = 1f;
                audio.minDistance = 1f;
                audio.maxDistance = 50f;
                if (Plugin.ambienceAudio != null) audio.clip = Plugin.ambienceAudio;

                goober.AddComponent<Movement>().startmovinguhidkman();
                nextSpawnTime = Time.time + 0.75f;
            }
        }

        public class Movement : MonoBehaviour
        {
            private Transform target;
            private float spawnTime;

            public void startmovinguhidkman()
            {
                target = GorillaTagger.Instance.offlineVRRig.transform;
                spawnTime = Time.time;
                StartCoroutine(FollowPlayer());
            }

            private IEnumerator FollowPlayer()
            {
                while (true)
                {
                    transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * 1.5f);
                    transform.LookAt(target, Vector3.up);
                    transform.Rotate(0f, 180f, 0f);

                    if (Time.time > spawnTime + 4f)
                    {
                        if (Vector3.Distance(transform.position, target.position) < 1.7f)
                        {
                            Application.Quit();
                        }
                    }
                }
            }
        }
    }
}