using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace carServiceApp.My_Classes
{
    class termsAgreementFragment :DialogFragment
    {
        private View     view;
        private TextView termsText;
        private Button   termsButton;
        private CheckBox termsCheckBox;
        private static bool checkBoxChecked;

        public event EventHandler<onTermsAgreementChosenArgs> OnTermsAgreementChosen;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view         = inflater.Inflate(Resource.Layout.termsAgreement, container, false);

            termsText    = view.FindViewById<TextView>(Resource.Id.termsText);
            termsButton   = view.FindViewById<Button>(Resource.Id.confirmTerms);
            termsCheckBox = view.FindViewById<CheckBox>(Resource.Id.termsCheckBox);

            termsText.Text = "Aplikacija sprema podatke koje upisujete prilikom registracije i prilikom kreiranja sastanka (ime, prezime, adresa, broj telefona, email). Navedeni podaci " +
                "koriste se isključivo u svrhu boljeg i lakšeg funkcioniranja autoservisa, odnosno u slučaju potrebne vučne službe ili dostave automobila nakon završetka servisa. " +
                "Podaci se neće koristiti za bilo kakve druge svrhe niti se prikupljaju bilo kakvi drugi podaci osim onih koje sami predajete putem aplikacije, a koji su gore navedeni. " +
                "Oznakom kvačice 'slažem se' pristajete na spremanje vaših podataka u bazu podataka autoservisa.";

            termsButton.Click += TermsButton_Click;
            termsCheckBox.CheckedChange += TermsCheckBox_CheckedChange;         

            return view;
        }

        private void TermsCheckBox_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            checkBoxChecked = e.IsChecked;
        }

        private void TermsButton_Click(object sender, EventArgs e)
        {
            if (checkBoxChecked)
            {
                OnTermsAgreementChosen.Invoke(this, new onTermsAgreementChosenArgs(checkBoxChecked));
                this.Dismiss();
            }
            else
            {
                OnTermsAgreementChosen.Invoke(this, new onTermsAgreementChosenArgs(checkBoxChecked, true));
                this.Dismiss();
            }
           
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            //Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            Dialog.Window.SetTitle("Uvjeti korištenja");
            base.OnActivityCreated(savedInstanceState);
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            if (!checkBoxChecked)
            {
                OnTermsAgreementChosen.Invoke(this, new onTermsAgreementChosenArgs(checkBoxChecked, true));
            }
            base.OnDismiss(dialog);
        }


    }

    public class onTermsAgreementChosenArgs
    {
        public bool termsAccepted { get; set; }
        public bool closeDIalog   { get; set; }

        public onTermsAgreementChosenArgs(bool userAnswer, bool close)
        {
            termsAccepted = userAnswer;
            closeDIalog = close;
        }
        public onTermsAgreementChosenArgs(bool userAnswer)
        {
            termsAccepted = userAnswer;
        }
    }
}