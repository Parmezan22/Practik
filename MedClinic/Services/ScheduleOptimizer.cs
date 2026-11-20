using System;
using System.Collections.Generic;
using System.Linq;

namespace MedClinic.Services
{
    /// <summary>Интервал рабочего дня в минутах от 00:00: [StartMinute; EndMinute).</summary>
    public class TimeWindow
    {
        public TimeWindow() { }

        public TimeWindow(int startMinute, int endMinute)
        {
            StartMinute = startMinute;
            EndMinute = endMinute;
        }

        public int StartMinute { get; set; }
        public int EndMinute { get; set; }

        public int Length
        {
            get { return EndMinute - StartMinute; }
        }

        public override string ToString()
        {
            return TimeRange.Format(StartMinute) + "–" + TimeRange.Format(EndMinute);
        }
    }

    /// <summary>Предложенное время приёма и окно, из которого оно взято.</summary>
    public class SlotSuggestion
    {
        public int StartMinute { get; set; }
        public int EndMinute { get; set; }

        /// <summary>Свободное окно расписания, в которое помещён приём.</summary>
        public TimeWindow Window { get; set; }

        public string StartText
        {
            get { return TimeRange.Format(StartMinute); }
        }

        public override string ToString()
        {
            return TimeRange.Format(StartMinute) + "–" + TimeRange.Format(EndMinute);
        }
    }

    /// <summary>
    /// Оптимизация расписания врача (модуль В).
    /// Занятые интервалы дня объединяются, между ними находятся свободные окна.
    /// Приём ставится по правилу «наилучшего подходящего окна» (best fit):
    /// выбирается самое короткое окно, в которое помещается вся длительность услуг,
    /// и приём ставится в его начало. Так в расписании остаётся меньше коротких
    /// «дыр», а длинные окна сохраняются для долгих приёмов.
    /// Для сравнения есть правило «первого подходящего окна» (first fit).
    /// </summary>
    public static class ScheduleOptimizer
    {
        /// <summary>Начало рабочего дня клиники, 08:00.</summary>
        public const int WorkStart = 8 * 60;

        /// <summary>Конец рабочего дня клиники, 20:00.</summary>
        public const int WorkEnd = 20 * 60;

        /// <summary>Шаг сетки времени: приём начинается в минуту, кратную 5.</summary>
        public const int Step = 5;

        /// <summary>
        /// Свободные окна рабочего дня. Занятые интервалы сортируются и объединяются,
        /// пересекающиеся и соприкасающиеся записи считаются одним блоком.
        /// </summary>
        public static List<TimeWindow> FreeWindows(IEnumerable<TimeWindow> busy, int dayStart, int dayEnd)
        {
            var result = new List<TimeWindow>();
            if (dayEnd <= dayStart) return result;

            var ordered = (busy ?? Enumerable.Empty<TimeWindow>())
                .Where(b => b != null && b.EndMinute > b.StartMinute)
                .OrderBy(b => b.StartMinute)
                .ToList();

            var cursor = dayStart;
            foreach (var block in ordered)
            {
                if (block.EndMinute <= cursor) continue;         // блок целиком раньше курсора
                if (block.StartMinute >= dayEnd) break;           // дальше только конец дня

                if (block.StartMinute > cursor)
                    result.Add(new TimeWindow(cursor, Math.Min(block.StartMinute, dayEnd)));

                cursor = Math.Max(cursor, block.EndMinute);
                if (cursor >= dayEnd) break;
            }

            if (cursor < dayEnd)
                result.Add(new TimeWindow(cursor, dayEnd));

            return result;
        }

        /// <summary>Первое по времени окно, куда помещается приём (first fit).</summary>
        public static SlotSuggestion FirstFit(IEnumerable<TimeWindow> busy, int duration, int notBefore)
        {
            return Candidates(busy, duration, notBefore).FirstOrDefault();
        }

        /// <summary>
        /// Самое короткое подходящее окно (best fit); при равной длине - более раннее.
        /// Возвращает null, если в этот день приём не помещается.
        /// </summary>
        public static SlotSuggestion BestFit(IEnumerable<TimeWindow> busy, int duration, int notBefore)
        {
            return Candidates(busy, duration, notBefore)
                .OrderBy(c => c.Window.Length)
                .ThenBy(c => c.StartMinute)
                .FirstOrDefault();
        }

        /// <summary>Доля занятого рабочего времени врача, от 0 до 1.</summary>
        public static double Utilization(IEnumerable<TimeWindow> busy, int dayStart, int dayEnd)
        {
            var day = dayEnd - dayStart;
            if (day <= 0) return 0;

            var free = FreeWindows(busy, dayStart, dayEnd).Sum(w => w.Length);
            return (double)(day - free) / day;
        }

        /// <summary>Округляет минуту вверх до шага сетки.</summary>
        public static int AlignUp(int minute)
        {
            if (minute <= 0) return 0;
            return (minute + Step - 1) / Step * Step;
        }

        /// <summary>Все окна, в которые помещается приём, с временем начала в каждом.</summary>
        private static IEnumerable<SlotSuggestion> Candidates(IEnumerable<TimeWindow> busy, int duration, int notBefore)
        {
            if (duration <= 0) duration = 30;
            var dayStart = AlignUp(Math.Max(WorkStart, notBefore));

            foreach (var window in FreeWindows(busy, dayStart, WorkEnd))
            {
                var start = AlignUp(window.StartMinute);
                if (start + duration <= window.EndMinute)
                {
                    yield return new SlotSuggestion
                    {
                        StartMinute = start,
                        EndMinute = start + duration,
                        Window = window
                    };
                }
            }
        }
    }
}
