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
        private View view;
        private TextView termsText;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.termsAgreement, container, false);
            termsText = view.FindViewById<TextView>(Resource.Id.termsText);

            termsText.Text = "Aplikacija sprema podatke koje upisujete prilikom registracije i prilikom kreiranja sastanka (ime, prezime, adresa, broj telefona, email). Navedeni podaci" +
                "koriste se isključivo u svrhu boljeg i lakšeg funkcioniranja autoservisa, odnosno u slučaju potrebne vučne službe ili dostave automobila nakon završetka servisa. " +
                "Podaci se neće koristiti za bilo kakve druge svrhe niti se prikupljaju bilo kakvi drugi podaci osim onih koje sami predajete putem aplikacije, a koji su gore navedeni. " +
                "Oznakom kvačice 'slažem se' pristajete na spremanje vaših podataka u bazu podataka autoservisa.";

            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }

    }
}