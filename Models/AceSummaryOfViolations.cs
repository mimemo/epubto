using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Reactive.Bindings;

namespace epubto.Models
{

    public class AceViolationCount : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string RuleSet { get; set; } = string.Empty;

        public ReactivePropertySlim<int> Critical { get; set; } = new ReactivePropertySlim<int>(0);
        public ReactivePropertySlim<int> Serious { get; set; } = new ReactivePropertySlim<int>(0);
        public ReactivePropertySlim<int> Moderate { get; set; } = new ReactivePropertySlim<int>(0);
        public ReactivePropertySlim<int> Minor { get; set; } = new ReactivePropertySlim<int>(0);
        public ReactivePropertySlim<int> Total { get; set; } = new ReactivePropertySlim<int>(0);

        public AceViolationCount(string tagname)
        {
            this.RuleSet = tagname;
        }

        private const StringComparison CMPIGNORE = StringComparison.CurrentCultureIgnoreCase;

        public void CountUp(string impactVal)
        {

            if (string.Equals("critical", impactVal, CMPIGNORE))
            {
                this.Serious.Value++;
            }

            if (string.Equals("serious", impactVal, CMPIGNORE))
            {
                this.Serious.Value++;
            }

            if (string.Equals("moderate", impactVal, CMPIGNORE))
            {
                this.Moderate.Value++;
            }

            if (string.Equals("minor", impactVal, CMPIGNORE))
            {
                this.Minor.Value++;
            }

            this.Total.Value = this.Critical.Value + this.Serious.Value + this.Moderate.Value + this.Minor.Value;

        }
    }

    /// <summary>
    /// Summary of violations
    /// </summary>
    public class AceSummaryOfViolations
    {
        public ReactiveProperty<AceViolationCount> WCAG20A { get; set; } = new ReactiveProperty<AceViolationCount>(new AceViolationCount("wcag2a"));
        public ReactiveProperty<AceViolationCount> WCAG20AA { get; set; } = new ReactiveProperty<AceViolationCount>(new AceViolationCount("wcag2aa"));
        public ReactiveProperty<AceViolationCount> EPUB { get; set; } = new ReactiveProperty<AceViolationCount>(new AceViolationCount("EPUB"));
        public ReactiveProperty<AceViolationCount> BestPractice { get; set; } = new ReactiveProperty<AceViolationCount>(new AceViolationCount("best-practice"));
        public ReactiveProperty<AceViolationCount> Other { get; set; } = new ReactiveProperty<AceViolationCount>(new AceViolationCount("Other"));
        public ReactiveProperty<AceViolationCount> Total { get; set; } = new ReactiveProperty<AceViolationCount>(new AceViolationCount("Total"));

        private readonly static StringComparer CMPRIGNORE = StringComparer.CurrentCultureIgnoreCase;

        public void Add(string impactValue, List<string> tags)
        {

            if (tags.Contains("wcag2a", CMPRIGNORE))
            {
                this.WCAG20A.Value.CountUp(impactValue);
            }
            else if (tags.Contains("wcag2aa", CMPRIGNORE))
            {
                this.WCAG20AA.Value.CountUp(impactValue);
            }
            else if (tags.Contains("EPUB", CMPRIGNORE))
            {
                this.EPUB.Value.CountUp(impactValue);
            }
            else if (tags.Contains("best-practice", CMPRIGNORE))
            {
                this.BestPractice.Value.CountUp(impactValue);
            }
            else
            {
                this.Other.Value.CountUp(impactValue);
            }
        }

    }
}
