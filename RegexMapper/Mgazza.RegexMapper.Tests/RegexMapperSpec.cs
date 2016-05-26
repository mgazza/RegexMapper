using System.Linq;
using Machine.Specifications;
using Shouldly;
using It = Machine.Specifications.It;

namespace Mgazza.RegexMapper.Tests
{
    public class RegexMapperSpec
    {
        private const string ICalRegexString =
            "(?:FREQ=(?<Freq>DAILY|WEEKLY|SECONDLY|MINUTELY|HOURLY|DAILY|WEEKLY|MONTHLY|YEARLY);?)?" +
             "(?:BYDAY=(?<ByDay>[-1-9A-Z,]+);?)?" +
             "(?:COUNT=(?<Count>[0-9]+);?)?" +
             "(?:INTERVAL=(?<Interval>[0-9]+);?)?" +
             "(?:BYMONTH=(?<ByMonth>[0-9]+);?)?" +
             "(?:BYMONTHDAY=(?<ByMonthDay>[0-9]+);?)?" +
             "(?:WKST=(?<WeekStart>[A-Z]+);?)?" +
             "(?:UNTIL=(?<Unit>[0-9TZ+:]+);?)?";



        private static RegexMapper icalMapper = new RegexMapper(ICalRegexString);
        [RegexMapper(ICalRegexString)]
        public class TestClass
        {
            public string Freq { get; set; }

            public int? Count { get; set; }

            public int? Interval { get; set; }

            public string ByDay { get; set; }

            public int? ByMonth { get; set; }

            public int? ByMonthDay { get; set; }

            public string WeekStart { get; set; }

            public string Until { get; set; }
        }


        public class When_binding_an_ical_string_to_a_class_using_mapper_map
        {
            private const string IcalString = "FREQ=WEEKLY;BYDAY=MO,TU,WE,TH,FR,SA,SU;WKST=MO";

            private static TestClass testClass;

            public Because _of = () =>
            {
                testClass = RegexMapper.Map<TestClass>(IcalString);
            };

            public It _should_have_mapped_weekly = () => testClass.Freq.ShouldBe("WEEKLY");
            public It _should_have_mapped_by_day = () => testClass.ByDay.ShouldBe("MO,TU,WE,TH,FR,SA,SU");
            public It _should_have_mapped_week_start = () => testClass.WeekStart.ShouldBe("MO");

        }

        public class When_binding_an_ical_string_to_a_class_using_mapper_apply
        {
            private const string IcalString = "FREQ=WEEKLY;BYDAY=MO,TU,WE,TH,FR,SA,SU;WKST=MO";

            private static TestClass testClass;

            public Because _of = () =>
            {
                testClass = new TestClass();

                icalMapper.Apply(IcalString, testClass, typeof(TestClass).GetProperties().ToList());
            };

            public It _should_have_mapped_weekly = () => testClass.Freq.ShouldBe("WEEKLY");
            public It _should_have_mapped_by_day = () => testClass.ByDay.ShouldBe("MO,TU,WE,TH,FR,SA,SU");
            public It _should_have_mapped_week_start = () => testClass.WeekStart.ShouldBe("MO");
        }
    }
}
