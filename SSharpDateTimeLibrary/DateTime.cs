using System;
using System.Text;
using Crestron.SimplSharp;                          				// For Basic SIMPL# Classes

namespace SSharpDateTimeLibrary
	{
	public static class DateTimePrecise
		{
		private static int m_offset = 0;
		private static int m_lastTickCount = 0;
		private static long m_tickBase = 0;

		internal static int Offset
			{
			get { return m_offset; }
			}

		static DateTimePrecise ()
			{
			int s = DateTime.Now.Second;
			while (true)
				{
				int s2 = DateTime.Now.Second;
				// wait for a rollover 
				if (s != s2)
					{
					m_offset = CrestronEnvironment.TickCount % 1000;
					break;
					}
				}
			}

		public static DateTime Now
			{
			get
				{
				// find where we are based on the os tick 
				int tick = CrestronEnvironment.TickCount % 1000;
				// calculate our ms shift from our base m_offset 
				int ms = (tick >= m_offset) ? (tick - m_offset) : (1000 - (m_offset - tick));
				// build a new DateTime with our calculated ms 
				// we use a new DateTime because some devices fill ms with a non-zero garbage value 
				DateTime now = DateTime.Now;
				return new DateTime (now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, ms);
				}
			}
		public static DateTime UtcNow
			{
			get
				{
				// find where we are based on the os tick 
				int tick = CrestronEnvironment.TickCount % 1000;
				// calculate our ms shift from our base m_offset 
				int ms = (tick >= m_offset) ? (tick - m_offset) : (1000 - (m_offset - tick));
				// build a new DateTime with our calculated ms 
				// we use a new DateTime because some devices fill ms with a non-zero garbage value 
				DateTime utcNow = DateTime.UtcNow;
				return new DateTime (utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, utcNow.Second, ms);
				}
			}

		public static void Calibrate (int seconds)
			{
			int s = DateTimePrecise.Now.Second;
			int sum = 0;
			int remaining = seconds;
			while (remaining > 0)
				{
				DateTime dt = DateTimePrecise.Now;
				int s2 = dt.Second;
				if (s != s2)
					{
					remaining--;
					// store the offset from zero 
					sum += (dt.Millisecond > 500) ? (dt.Millisecond - 1000) : dt.Millisecond;
					s = dt.Second;
					}
				}
			// adjust the offset by the average deviation from zero (round to the integer farthest from zero) 
			if (sum < 0)
				{
				m_offset += (int)Math.Floor (sum / (float)seconds);
				}
			else
				{
				m_offset += (int)Math.Ceiling (sum / (float)seconds);
				}
			}

		public static long TickCountLong
			{
			get
				{
				int tickCount = CrestronEnvironment.TickCount;
				if (tickCount < m_lastTickCount)
					m_tickBase += (long)Int32.MaxValue + 1;
				m_lastTickCount = tickCount;
				return tickCount + m_tickBase;
				}
			}
		}

	public static class DateTimeOffsetPrecise
		{
		public static DateTimeOffset Now
			{
			get
				{
				int offset = DateTimePrecise.Offset;

				// find where we are based on the os tick 
				int tick = CrestronEnvironment.TickCount % 1000;
				// calculate our ms shift from our base m_offset 
				int ms = (tick >= offset) ? (tick - offset) : (1000 - (offset - tick));
				// build a new DateTime with our calculated ms 
				// we use a new DateTime because some devices fill ms with a non-zero garbage value 
				DateTimeOffset now = DateTimeOffset.Now;
				return new DateTimeOffset (now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, ms, now.Offset);
				}
			}
		public static DateTimeOffset UtcNow
			{
			get
				{
				int offset = DateTimePrecise.Offset;

				// find where we are based on the os tick 
				int tick = CrestronEnvironment.TickCount % 1000;
				// calculate our ms shift from our base m_offset 
				int ms = (tick >= offset) ? (tick - offset) : (1000 - (offset - tick));
				// build a new DateTime with our calculated ms 
				// we use a new DateTime because some devices fill ms with a non-zero garbage value 
				DateTimeOffset utcNow = DateTimeOffset.UtcNow;
				return new DateTimeOffset (utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, utcNow.Second, ms, utcNow.Offset);
				}
			}
		}
	}
