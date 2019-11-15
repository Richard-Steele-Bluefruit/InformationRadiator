using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace InformationRadiator
{
    struct HolidayInfo
    {
        public HolidayInfo(string _name, DateTime _firstDay, DateTime _lastDay, Color _colour) { Name = _name; firstDay = _firstDay; LastDay = _lastDay; Colour = _colour; SingleDay = false; }
        public HolidayInfo(string _name, DateTime _firstDay, Color _colour) { Name = _name; firstDay = _firstDay; LastDay = _firstDay; Colour = _colour; SingleDay = true; }

        string Name;
        DateTime firstDay, LastDay;
        Color Colour;
        bool SingleDay;
    }

    class HolidayRenderer
    {
        private List<HolidayInfo> mHolidays;
        private Color[] mColours;
        private int mColourIndex;

        public HolidayRenderer()
        {
            mHolidays = new List<HolidayInfo>();
            mColours = new Color[]
            {
                Color.Red, Color.Purple, Color.SeaGreen, Color.Teal, Color.Turquoise, Color.Yellow
            };
            mColourIndex = 0;
        }

        public void AddHoliday(string _name, DateTime _startDay)
        {
            mHolidays.Add(new HolidayInfo(_name, _startDay, GetNextColour()));
        }

        public void AddHoliday(string _name, DateTime _startDay, DateTime _lastDay)
        {
            mHolidays.Add(new HolidayInfo(_name, _startDay, _lastDay, GetNextColour()));
        }

        private Color GetNextColour()
        {
            if (mColourIndex == mColours.Length - 1)
                mColourIndex = 0;
            else
                mColourIndex++;

            return mColours[mColourIndex];
        }

        public void Draw()
        {

        }
    }
}
