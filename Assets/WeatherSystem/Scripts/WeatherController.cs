﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace weatherSystem {
    public class WeatherController : MonoBehaviour {
        #region public Atributes
		[Tooltip("For testing purposes. If checked debug messages will be writen on the console")]
		public bool debug; 

		[Header ("Symulation parameters:")]
		public int update_frequency = 60;
		public float previous_state_influence;
		public float biome_influence;
		
		[Header ("Random Generation Values:")]
		public float probability_stdDev;
        public float intensity_stdDev;
		public float temperature_stdDev;

        [Header("Weather Visualization (Particle Systems):")]
        public CloudController cloudController;
        public ParticleController rainController;
        public ParticleController snowController;
        //public ParticleSystem lightningSystem;
        
		#endregion public Atributes
		
        #region private Atributes
        private CentralClock clock;
        private Time lastUpdate;
		private Season currentSeason;
		private WeatherState currentState;
		private WeatherState previousState;
        public Biome biome; //TODO: cambiar esto en el futuro

        #endregion private Atributes 

		private WeatherState GetNextState(WeatherState currentState) {
            if(currentSeason == null) {
                Debug.LogError("WEATHERCONTROLLER: Current Season is set to null set to null");
                Debug.Break();
            }

            //if (debug) {
            //    Debug.Log("WEATHERCONTROLLER: Calculating the next state.... \n" +
            //        "Current state: " + currentState.ToString() + "\n" + 
            //        "current season: " + currentSeason);
            //}
            //calculating the clouds:
            double cloudProbability = biome.GetProbability(currentSeason, Biome.CLOUDS)*biome_influence + currentState.GetClouds()/10.0*previous_state_influence + GenerateNormalRandom(0, probability_stdDev) ;
            //if (debug) {
            //    Debug.Log("Cloud Calculations.... ");
            //    Debug.Log("Probability calculated: " + cloudProbability);
            //    Debug.Break();
            //}
            int cloudIntensity = 0;
            if (cloudProbability>= Random.Range(0, 1)) {
                cloudIntensity = (int) (Mathf.Max(Mathf.Min( (biome.GetIntensity(currentSeason, Biome.CLOUDS)*biome_influence + currentState.GetClouds()*previous_state_influence + GenerateNormalRandom(0, intensity_stdDev)), 10), 0));
                //if (debug) {
                //    Debug.Log("Intensity calculated: " + cloudIntensity);
                //    Debug.Break();
                //}
            }else if (debug) {
                Debug.Log("The probability was lower than the roll, therefore intensity is set to 0");
            }
            
            //calculating rain:
            double rainProbability = biome.GetProbability(currentSeason, Biome.RAIN) * biome_influence + currentState.GetRain()/10 * previous_state_influence + GenerateNormalRandom(0, probability_stdDev);
            //if (debug) {
            //    Debug.Log("Rain Calculations.... ");
            //    Debug.Log("Probability calculated: " + rainProbability);
            //    Debug.Break();
            //}
            int rainIntensity = 0;
            //if (rainProbability >= Random.Range(0, 1)) {
            //    rainIntensity = (int)(Mathf.Max(Mathf.Min((biome.GetIntensity(currentSeason, Biome.RAIN) * biome_influence + ( cloudIntensity*0.2f + currentState.GetRain()*0.8f  )* previous_state_influence + GenerateNormalRandom(0, intensity_stdDev)), cloudIntensity), 0));
            //    if (debug) {
            //        Debug.Log("Intensity calculated: " + rainIntensity);
            //        Debug.Break();
            //    }
            //}

            //calculating snow
            double snowProbability = biome.GetProbability(currentSeason, Biome.SNOW) * biome_influence + currentState.GetSnow() / 10.0 * previous_state_influence + GenerateNormalRandom(0, probability_stdDev);
            //if (debug) {
            //    Debug.Log("Snow Calculations.... ");
            //    Debug.Log("Probability calculated: " + snowProbability);
            //    Debug.Break();
            //}
            int snowIntensity = 0;
            if (snowProbability >= Random.Range(0, 1)) {
                snowIntensity = (int)(Mathf.Max(Mathf.Min((biome.GetIntensity(currentSeason, Biome.SNOW) * biome_influence + (cloudIntensity*0.2f + currentState.GetSnow()*0.8f)* previous_state_influence + GenerateNormalRandom(0, intensity_stdDev)), cloudIntensity), 0));
                //if (debug) {
                //    Debug.Log("Intensity calculated: " + snowIntensity);
                //    Debug.Break();
                //}
            }
            //coherency test: no rain and snow at the same time
            if (rainIntensity>0 && snowIntensity > 0) {
                //if (debug) {
                //    Debug.Log("snow and rain detected at the same time, the one with the lowest probability will be set to 0");
                //}
                if (rainProbability >= snowProbability) {
                    snowIntensity = 0;
                    if (debug) {
                        Debug.Log("rain has higher probabilities than snow, therfore snow intensity has been set to 0"); //TODO: revisar esta frase
                    }
                } else {
                    rainIntensity = 0;
                    if (debug) {
                        Debug.Log("snow has higher probabilities than rain, therfore rain intensity has been set to 0");
                    }
                }
            }

            //storm calculations
            double LightningProbability = biome.GetProbability(currentSeason, Biome.SNOW) * biome_influence + currentState.GetLightning() / 10.0 * previous_state_influence + GenerateNormalRandom(0, probability_stdDev);
            //if (debug) {
            //    Debug.Log("Lightning Calculations.... ");
            //    Debug.Log("Probability calculated: " + LightningProbability);
            //    Debug.Break();
            //}
            int LightningIntensity = 0;
            if (LightningProbability >= Random.Range(0, 1)) {
                LightningIntensity = (int)(Mathf.Max(Mathf.Min((biome.GetIntensity(currentSeason, Biome.SNOW) * biome_influence + (cloudIntensity * 0.2f +currentState.GetLightning()*0.8f )* previous_state_influence + GenerateNormalRandom(0, intensity_stdDev)), rainIntensity), 0));
                //if (debug) {
                //    Debug.Log("Intensity calculated: " + LightningIntensity);
                //    Debug.Break();
                //}
            }

            WeatherState result = new WeatherState(cloudIntensity, rainIntensity, LightningIntensity, snowIntensity);

            return result;
        }

        private void VisualizeWeatherState(WeatherState currentState) {
            cloudController.SetIntensity(currentState.GetClouds());
            rainController.SetIntensity(currentState.GetRain());
            snowController.SetIntensity(currentState.GetSnow());
            //TODO: repetir para rain, snow, lightning
        }
        public static float GenerateNormalRandom(float mu, float sigma) {
            float rand1 = Random.Range(0.0f, 1.0f);
            float rand2 = Random.Range(0.0f, 1.0f);

            float n = Mathf.Sqrt(-2.0f * Mathf.Log(rand1)) * Mathf.Cos((2.0f * Mathf.PI) * rand2);

            return (mu + sigma * n);
        }

        public override string ToString() {
            string result = "State " + lastUpdate.ToString() + "  : \n\t" +
                currentState.ToString();
            return result;
        }

        void Awake() {
            //setting clock:
            clock = GameObject.FindGameObjectWithTag("Clock").GetComponent<CentralClock>();
			if (clock == null) {
                Debug.LogError("WEATHERCONTROLLER: No clock could be found, please remember to tag the object with the CentralClock script with the tag 'Clock' ");
                Debug.Break();
            }
			
			//no more than 1 weathercontroller
			int numberOfControllers = GameObject.FindGameObjectsWithTag("WeatherController").Length;
			if(numberOfControllers <1){
				Debug.LogError("WEATHERCONTROLLER: the GameObject containing the WeatherController script needs to be taged as 'WeatherController'");
			}
			if(numberOfControllers >1){
				Debug.LogError("WEATHERCONTROLLER: multiple GameObjects found with the tag 'WeatherController', there should be only one"); 
			}
			
			//checking state multipliers
			if(previous_state_influence + biome_influence != 1){
				Debug.LogError("WEATHERCONTROLLER: There is an error with the simulation parameters, previous_state_influence and biome_influence need to sum 1"); //TODO: esta frase esta bien escrita???
			}
			
			//checking ParticleSystems
            //if(rainSystem == null) {
            //    Debug.LogError("WEATHERCONTROLLER: rain system reference can not be null");
            //}
            //if (snowSystem == null) {
            //    Debug.LogError("WEATHERCONTROLLER: snow system reference can not be null");
            //}
            //if (lightningSystem == null) {
            //    Debug.LogError("WEATHERCONTROLLER: lighning system reference can not be null");
            //}
		}

        // Use this for initialization
        void Start() {
            // get season
            currentSeason = clock.GetSeason();

            currentState = new WeatherState(0,0,0,0);
            currentState.ToString();

            lastUpdate = clock.getCurrentTime().Clone();
        }

        // Update is called once per frame
        void Update() {
            Time currentTime = clock.getCurrentTime();
            
            if(lastUpdate.SecondsBetween(currentTime) >= (update_frequency * 60)) {
				currentSeason = clock.GetSeason();
                currentState = GetNextState(currentState);
                VisualizeWeatherState(currentState);
                if (debug) {
                    Debug.Log("WEATHERCONTROLLER: A new Weather has been calculated: ");
					Debug.Log(ToString());
					Debug.Break();
                }
                lastUpdate = currentTime.Clone();
            }

        }
    }
}
