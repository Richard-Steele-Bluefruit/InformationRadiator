using System;
using PresenterCommon;
using PresenterCommon.Configuration;

namespace Decorations.Presenters
{
    public class DecorationsPresenter
    {
        #region Steve Quotes

        private readonly string[] _quotes =
        {
            "I can load a picture up of my pussys",
            "I was going to say Claire jumped on top of me this morning...",
            "That Harvey is a real queen",
            "I had a room of thirty female sports teachers. It was bizarre",
            "Hopefully that is enough toilet talk for today.",
            "You've got to spread yourself out mate",
            "It's nearly time for Secret Sue",
            "You put your hand in the box or something",
            "I know how to twat, yeah.",
            "Anna: Women are good at communication - Steve: Too much communications some time",
            "Steve: This parcel feels like Judith is buying shoes! - Anna: I'm not sure you're supposed to be fondling other peoples' parcels... - Steve: Oh well as long as it's only their parcels.",
            "Porn does it to mine regularly",
            "I sent the company accounts to my hair dresser.. they have the same first name as our accountant",
            "He reminds of someone I once saw in a video!!",
            "See, look! Hoodies for the boys and shopping bags for the girls!",
            "I often get misquoted!",
            "We have 6 people in a darkened room",
            "Share it out, I have some videos I could store on there!!",
            "Lance prodded me personally",
            "We haven't been involved at all because we haven't been turned on... uhhh told about it",
            "We want to show them that software isn't just a load of people like Colin sat around.",
            "Sorry, I've got a buzzing in my pocket",
            "Steve: Gonna head down to Spasda later with Clare. - Me: I'm sorry, WHAT did you just say? - Steve: Going Spasda. - Me: I'm...not sure you can call it that nowadays Steve.",
            "Anna if you smiled more then maybe more people would come to our stand",
            "Claire's out for the evening so I have to take advantage, porn on the big screen!",
            "You saw mine on Saturday Robert!....Chest I mean.",
            "Matt's just had a baby so he's a bit scatty at the moment",
            "Anna has a habit of eating all the chocolates",
            "When you say a fish talking in bad English, are you pointing at Andi, Pablo or Denzil?",
            "I can see where the fluids go in, but where do they come out."
        };

        public string[] SteveQuotes
        {
            get { return _quotes; }
        }

        #endregion Steve Quotes

        private bool _forceChristmas;
        private bool _forceEaster;

        private ITimer _steveQuotesTimer;
        private int _steveQuoteCounter;
        private int _steveQuoteTicks;
        private const int steveQuoteCounterDisplayMessage = 14;
        private const int steveQuoteCounterReset = 15;


        public DecorationsPresenter(IDayUpdateMonitor updateMonitor, InformationRadiatorItemConfiguration configuration)
        {
            _forceChristmas = false;
            _forceEaster = false;
            ParseConfiguration(configuration);
            updateMonitor.DayChanged += updateMonitor_DayChanged;

            _steveQuoteCounter = 0;
            _steveQuoteTicks = 0;
            _steveQuotesTimer = PresenterCommonFactory.Instance.CreateTimer(20000);
            _steveQuotesTimer.Tick += _steveQuotesTimer_Tick;
        }

        private void updateMonitor_DayChanged(object sender, EventArgs e)
        {
            OnUpdate();
        }

        private int IncrementWithMax(int value, int max)
        {
            value++;
            if(value >= max)
            {
                value = 0;
            }
            return value;
        }

        private void _steveQuotesTimer_Tick(object sender, EventArgs e)
        {
            if(!IsChristmas)
            {
                OnSteveQuoteUpdate(false);
                _steveQuoteCounter = 0;
                _steveQuoteTicks = 0;
            }

            _steveQuoteTicks = IncrementWithMax(_steveQuoteTicks, steveQuoteCounterReset);

            switch(_steveQuoteTicks)
            {
                case steveQuoteCounterDisplayMessage:
                    OnSteveQuoteUpdate(true, _quotes[_steveQuoteCounter]);
                    _steveQuoteCounter = IncrementWithMax(_steveQuoteCounter, _quotes.Length);
                    break;
                case 0:
                    OnSteveQuoteUpdate(false);
                    break;
            }
        }

        public class SteveQuoteEventArgs : EventArgs
        {
            public bool ShowSteveAngel { get; set; }
            public string Quote { get; set; }
        }

        public event EventHandler<SteveQuoteEventArgs> SteveQuoteUpdate;
        protected void OnSteveQuoteUpdate(bool show, string quote = "")
        {
            var handler = SteveQuoteUpdate;
            if(handler != null)
            {
                SteveQuoteUpdate(this, new SteveQuoteEventArgs() { ShowSteveAngel = show, Quote = quote });
            }
        }

        private void ParseConfiguration(InformationRadiatorItemConfiguration configuration)
        {
            foreach (var item in configuration)
            {
                switch (item.ID.ToLower())
                {
                    case "forcechristmas":
                        bool forceChristmas;
                        if (bool.TryParse(item.Value, out forceChristmas))
                            _forceChristmas = forceChristmas;
                        break;
                    case "forceeaster":
                        bool forceEaster;
                        if (bool.TryParse(item.Value, out forceEaster))
                            _forceEaster = forceEaster;
                        break;
                }
            }
        }


        public event EventHandler Update;
        protected void OnUpdate()
        {
            var handler = Update;
            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public bool IsChristmas
        {
            get
            {
                var now = DotNet.Instance.Now;
                return _forceChristmas || (now.Month == 12 && now.Day >= 3);
            }
        }

        public bool IsEaster
        {
            get
            {
                var now = DotNet.Instance.Now;
                return _forceEaster || (now.Month == 3 && now.Day >= 20) || (now.Month == 4 && now.Day <= 8);
            }
        }
    }
}
