using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.LanguageTranslator.V3;
using IBM.Watson.LanguageTranslator.V3.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LangaugeTranslatorDemo
{
    public class LanguageTranslatorSample : MonoBehaviour
    {
        #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
        [Space(10)]
        [Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/langauge-translator/api\"")]
        [SerializeField]
        private string serviceUrl;
        [Tooltip("Text field to display the results of translation.")]
        public Text ResultsField;
        [Header("IAM Authentication")]
        [Tooltip("The IAM apikey.")]
        [SerializeField]
        private string iamApikey;
        [Header("Parameters")]
        // https://cloud.ibm.com/apidocs/language-translator#list-models
        [Tooltip("The translation model to use. See https://cloud.ibm.com/apidocs/language-translator#list-models.")]
        [SerializeField]
        private string translationModel;
        #endregion

        private LanguageTranslatorService languageTranslator;

        void Start()
        {
            if (string.IsNullOrEmpty(iamApikey))
                throw new IBMException("Please set the Language Translator iamApikey in the inspector.");
            if (string.IsNullOrEmpty(translationModel))
                throw new IBMException("Please set the translationModel in the inspector.");
            //  Start coroutine to create service
            StartCoroutine(CreateService());
        }

        private IEnumerator CreateService()
        {
            //  Create TokenOptions
            TokenOptions languageTranslatorTokenOptions = new TokenOptions()
            {
                IamApiKey = iamApikey
            };

            //  Create Credentials object
            Credentials languageTranslatorCredentials = new Credentials(
                iamTokenOptions: languageTranslatorTokenOptions,
                serviceUrl: serviceUrl
                );

            //  Yield here until we have IAM token data
            while (!languageTranslatorCredentials.HasIamTokenData())
                yield return null;

            //  Instantiate service
            languageTranslator = new LanguageTranslatorService("2019-04-09", languageTranslatorCredentials);
        }

        //  Call this method from ExampleStreaming
        public void Translate(string text)
        {
            //  Array of text to translate
            List<string> translateText = new List<string>();
            translateText.Add(text);

            //  Call to the service
            languageTranslator.Translate(OnTranslate, translateText, translationModel);
        }

        //  OnTranslate handler
        private void OnTranslate(DetailedResponse<TranslationResult> response, IBMError error)
        {
            //  Populate text field with TranslationOutput
            ResultsField.text = response.Result.Translations[0].TranslationOutput;
        }
    }
}