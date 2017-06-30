using AntMe.Deutsch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntMe.Player.Killerameise
{

    [Spieler(
        Volkname = "Killerameise",   // Hier kannst du den Namen des Volkes festlegen
        Vorname = "Pia",       // An dieser Stelle kannst du dich als Schöpfer der Ameise eintragen
        Nachname = ""       // An dieser Stelle kannst du dich als Schöpfer der Ameise eintragen
    )]

    [Kaste(
        Name = "Sammler",                  // Name der Berufsgruppe
        AngriffModifikator = -1,             // Angriffsstärke einer Ameise
        DrehgeschwindigkeitModifikator = 0, // Drehgeschwindigkeit einer Ameise
        EnergieModifikator = -1,             // Lebensenergie einer Ameise
        GeschwindigkeitModifikator = 0,     // Laufgeschwindigkeit einer Ameise
        LastModifikator = 1,                // Tragkraft einer Ameise
        ReichweiteModifikator = 0,          // Ausdauer einer Ameise
        SichtweiteModifikator = 1           // Sichtweite einer Ameise
    )]
	[Kaste(
		Name = "Kämpfer",                  // Name der Berufsgruppe
		AngriffModifikator = 1,             // Angriffsstärke einer Ameise
		DrehgeschwindigkeitModifikator = -1, // Drehgeschwindigkeit einer Ameise
		EnergieModifikator = 1,             // Lebensenergie einer Ameise
		GeschwindigkeitModifikator = 0,     // Laufgeschwindigkeit einer Ameise
		LastModifikator = -1,               // Tragkraft einer Ameise
		ReichweiteModifikator = -1,          // Ausdauer einer Ameise
		SichtweiteModifikator = 1           // Sichtweite einer Ameise
	)]
	public class KillerameiseKlasse : Basisameise
    {
        #region Kasten

        /// <summary>
        /// Jedes mal, wenn eine neue Ameise geboren wird, muss ihre Berufsgruppe
        /// bestimmt werden. Das kannst du mit Hilfe dieses Rückgabewertes dieser 
        /// Methode steuern.
        /// Weitere Infos unter http://wiki.antme.net/de/API1:BestimmeKaste
        /// </summary>
        /// <param name="anzahl">Anzahl Ameisen pro Kaste</param>
        /// <returns>Name der Kaste zu der die geborene Ameise gehören soll</returns>
        public override string BestimmeKaste(Dictionary<string, int> anzahl)
        {
			if (anzahl["Sammler"] < 50)
			{
				return "Sammler";
			}
			else
			{
				return "Kämpfer";
			}
            // Gibt den Namen der betroffenen Kaste zurück.
        }

		#endregion

		#region Fortbewegung

		/// <summary>
		/// Wenn die Ameise keinerlei Aufträge hat, wartet sie auf neue Aufgaben. Um dir das 
		/// mitzuteilen, wird diese Methode hier aufgerufen.
		/// Weitere Infos unter http://wiki.antme.net/de/API1:Wartet
		/// </summary>
		public override void Wartet()
        {
			GeheGeradeaus();
        }

        /// <summary>
        /// Erreicht eine Ameise ein drittel ihrer Laufreichweite, wird diese Methode aufgerufen.
        /// Weitere Infos unter http://wiki.antme.net/de/API1:WirdM%C3%BCde
        /// </summary>
        public override void WirdMüde()
        {
			GeheZuBau();
        }

        /// <summary>
        /// Wenn eine Ameise stirbt, wird diese Methode aufgerufen. Man erfährt dadurch, wie 
        /// die Ameise gestorben ist. Die Ameise kann zu diesem Zeitpunkt aber keinerlei Aktion 
        /// mehr ausführen.
        /// Weitere Infos unter http://wiki.antme.net/de/API1:IstGestorben
        /// </summary>
        /// <param name="todesart">Art des Todes</param>
        public override void IstGestorben(Todesart todesart)
        {
        }

        /// <summary>
        /// Diese Methode wird in jeder Simulationsrunde aufgerufen - ungeachtet von zusätzlichen 
        /// Bedingungen. Dies eignet sich für Aktionen, die unter Bedingungen ausgeführt werden 
        /// sollen, die von den anderen Methoden nicht behandelt werden.
        /// Weitere Infos unter http://wiki.antme.net/de/API1:Tick
        /// </summary>
        public override void Tick()
        {
			if (Ziel is Bau && AktuelleLast > 0 && GetragenesObst == null)
			{
				SprüheMarkierung(Richtung + 180);
			}
			if (AktuelleEnergie < MaximaleEnergie/2)
			{
				LasseNahrungFallen();
				GeheZuBau();
			}
        }

        #endregion

        #region Nahrung

        /// <summary>
        /// Sobald eine Ameise innerhalb ihres Sichtradius einen Apfel erspäht wird 
        /// diese Methode aufgerufen. Als Parameter kommt das betroffene Stück Obst.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:Sieht(Obst)"
        /// </summary>
        /// <param name="obst">Das gesichtete Stück Obst</param>
        public override void Sieht(Obst obst)
        {
			if (!(Ziel is Wanze)&& Kaste == "Sammler")
			{
				if (AktuelleLast == 0 && BrauchtNochTräger(obst))
				{
					GeheZuZiel(obst);
					int entfernung, richtung;
					entfernung = Koordinate.BestimmeEntfernung(this, obst);
					richtung = Koordinate.BestimmeRichtung(this, obst);
					SprüheMarkierung(richtung, entfernung);
				}
			}
		}

        /// <summary>
        /// Sobald eine Ameise innerhalb ihres Sichtradius einen Zuckerhügel erspäht wird 
        /// diese Methode aufgerufen. Als Parameter kommt der betroffene Zuckerghügel.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:Sieht(Zucker)"
        /// </summary>
        /// <param name="zucker">Der gesichtete Zuckerhügel</param>
        public override void Sieht(Zucker zucker)
        {
			
			if (!(Ziel is Wanze)&& Kaste == "Sammler")
			{
				if (AktuelleLast == 0)
				{
					GeheZuZiel(zucker);
					int entfernung, richtung;
					entfernung = Koordinate.BestimmeEntfernung(this, zucker);
					richtung = Koordinate.BestimmeRichtung(this, zucker);
					SprüheMarkierung(richtung, entfernung);
				}
			}
		}

        /// <summary>
        /// Hat die Ameise ein Stück Obst als Ziel festgelegt, wird diese Methode aufgerufen, 
        /// sobald die Ameise ihr Ziel erreicht hat. Ab jetzt ist die Ameise nahe genug um mit 
        /// dem Ziel zu interagieren.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:ZielErreicht(Obst)"
        /// </summary>
        /// <param name="obst">Das erreichte Stück Obst</param>
        public override void ZielErreicht(Obst obst)
        {
			if (BrauchtNochTräger(obst))
			{
				Nimm(obst);
				GeheZuBau();
			}
        }

        /// <summary>
        /// Hat die Ameise eine Zuckerhügel als Ziel festgelegt, wird diese Methode aufgerufen, 
        /// sobald die Ameise ihr Ziel erreicht hat. Ab jetzt ist die Ameise nahe genug um mit 
        /// dem Ziel zu interagieren.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:ZielErreicht(Zucker)"
        /// </summary>
        /// <param name="zucker">Der erreichte Zuckerhügel</param>
        public override void ZielErreicht(Zucker zucker)
        {
			Nimm(zucker);
			GeheZuBau();
        }



		#endregion
		#region Kommunikation

		/// <summary>
		/// Markierungen, die von anderen Ameisen platziert werden, können von befreundeten Ameisen 
		/// gewittert werden. Diese Methode wird aufgerufen, wenn eine Ameise zum ersten Mal eine 
		/// befreundete Markierung riecht.
		/// Weitere Infos unter "http://wiki.antme.net/de/API1:RiechtFreund(Markierung)"
		/// </summary>
		/// <param name="markierung">Die gerochene Markierung</param>
		public override void RiechtFreund(Markierung markierung)
		{
			if (Ziel == null)
			{
				DreheInRichtung(markierung.Information);
				GeheGeradeaus();
			}
			

		}


        /// <summary>
        /// So wie Ameisen unterschiedliche Nahrungsmittel erspähen können, entdecken Sie auch 
        /// andere Spielelemente. Entdeckt die Ameise eine Ameise aus dem eigenen Volk, so 
        /// wird diese Methode aufgerufen.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:SiehtFreund(Ameise)"
        /// </summary>
        /// <param name="ameise">Erspähte befreundete Ameise</param>
        public override void SiehtFreund(Ameise ameise)
        {

			if ((this.Kaste == "Kämpfer")  && 
				(this.AnzahlAmeisenDerSelbenKasteInSichtweite > 2) &&
				(ameise.Richtung == this.Richtung))
			{
				DreheInRichtung(Koordinate.BestimmeRichtung(this, ameise));
				GeheGeradeaus();
			}
        }

        /// <summary>
        /// So wie Ameisen unterschiedliche Nahrungsmittel erspähen können, entdecken Sie auch 
        /// andere Spielelemente. Entdeckt die Ameise eine Ameise aus einem befreundeten Volk 
        /// (Völker im selben Team), so wird diese Methode aufgerufen.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:SiehtVerb%C3%BCndeten(Ameise)"
        /// </summary>
        /// <param name="ameise">Erspähte verbündete Ameise</param>
        public override void SiehtVerbündeten(Ameise ameise)
        {
        }

        #endregion

        #region Kampf

        /// <summary>
        /// So wie Ameisen unterschiedliche Nahrungsmittel erspähen können, entdecken Sie auch 
        /// andere Spielelemente. Entdeckt die Ameise eine Ameise aus einem feindlichen Volk, 
        /// so wird diese Methode aufgerufen.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:SiehtFeind(Ameise)"
        /// </summary>
        /// <param name="ameise">Erspähte feindliche Ameise</param>
        public override void SiehtFeind(Ameise ameise)
		{
			LasseNahrungFallen();
			if (AnzahlAmeisenInSichtweite >= 1 && Kaste == "Kämpfer")
			{
				GreifeAn(ameise);
				int entfernung, richtung;
				entfernung = Koordinate.BestimmeEntfernung(this, ameise);
				richtung = Koordinate.BestimmeRichtung(this, ameise);
				SprüheMarkierung(richtung, entfernung);
			}
			else
			{
				GeheWegVon(ameise);
			}

		}

        /// <summary>
        /// So wie Ameisen unterschiedliche Nahrungsmittel erspähen können, entdecken Sie auch 
        /// andere Spielelemente. Entdeckt die Ameise eine Wanze, so wird diese Methode aufgerufen.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:SiehtFeind(Wanze)"
        /// </summary>
        /// <param name="wanze">Erspähte Wanze</param>
        public override void SiehtFeind(Wanze wanze)
        {
			LasseNahrungFallen();
			if (AnzahlAmeisenInSichtweite >= 4 && Kaste == "Kämpfer")
			{
				GreifeAn(wanze);
				int entfernung, richtung;
				entfernung = Koordinate.BestimmeEntfernung(this, wanze);
				richtung = Koordinate.BestimmeRichtung(this, wanze);
				SprüheMarkierung(richtung, entfernung);
			}
			else
			{
				GeheWegVon(wanze);
			}
        }

        /// <summary>
        /// Es kann vorkommen, dass feindliche Lebewesen eine Ameise aktiv angreifen. Sollte 
        /// eine feindliche Ameise angreifen, wird diese Methode hier aufgerufen und die 
        /// Ameise kann entscheiden, wie sie darauf reagieren möchte.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:WirdAngegriffen(Ameise)"
        /// </summary>
        /// <param name="ameise">Angreifende Ameise</param>
        public override void WirdAngegriffen(Ameise ameise)
        {
			if (Kaste == "Sammler")
			{
				LasseNahrungFallen();
				GreifeAn(ameise);
			}
			else if (Kaste == "Kämpfer")
			{
				if (AnzahlAmeisenInSichtweite * MaximaleEnergie > ameise.AktuelleEnergie)
				{
					GreifeAn(ameise);
				}
			}

		}

        /// <summary>
        /// Es kann vorkommen, dass feindliche Lebewesen eine Ameise aktiv angreifen. Sollte 
        /// eine Wanze angreifen, wird diese Methode hier aufgerufen und die Ameise kann 
        /// entscheiden, wie sie darauf reagieren möchte.
        /// Weitere Infos unter "http://wiki.antme.net/de/API1:WirdAngegriffen(Wanze)"
        /// </summary>
        /// <param name="wanze">Angreifende Wanze</param>
        public override void WirdAngegriffen(Wanze wanze)
        {
			if (Kaste == "Sammler")
			{
				LasseNahrungFallen();
				GeheWegVon(wanze);
			}
			else if (Kaste == "Kämpfer")
			{
				if (AnzahlAmeisenInSichtweite * MaximaleEnergie > wanze.AktuelleEnergie)
				{
					GreifeAn(wanze);
				}
			}
        }

        #endregion
    }
}
