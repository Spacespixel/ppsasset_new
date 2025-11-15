# SEO Performance Tracking Template

## Copy this into Google Sheets for monthly tracking

### 1. Monthly Summary Dashboard

```
Date: [MONTH/YEAR]

INDEXING STATUS
├─ Total URLs in Sitemap: 30+
├─ URLs Indexed in Google: [From Search Console]
├─ Indexing Rate: [Indexed / Total × 100]%
└─ New URLs Indexed This Month: [Number]

SEARCH PERFORMANCE
├─ Total Impressions: [From Search Console]
├─ Total Clicks: [From Search Console]
├─ Avg Click-Through Rate (CTR): [Clicks / Impressions × 100]%
├─ Avg Position: [From Search Console]
└─ Top 5 Keywords:
    1. [Keyword] - Position: [Rank] - CTR: [%]
    2. [Keyword] - Position: [Rank] - CTR: [%]
    3. [Keyword] - Position: [Rank] - CTR: [%]
    4. [Keyword] - Position: [Rank] - CTR: [%]
    5. [Keyword] - Position: [Rank] - CTR: [%]

ORGANIC TRAFFIC (From Analytics)
├─ Organic Sessions: [Number]
├─ Organic Users: [Number]
├─ Bounce Rate: [%]
├─ Avg Session Duration: [Minutes]
└─ Pages per Session: [Number]

PAGE PERFORMANCE
├─ Top Landing Pages:
│   1. [URL] - Sessions: [#] - CTR: [%]
│   2. [URL] - Sessions: [#] - CTR: [%]
│   3. [URL] - Sessions: [#] - CTR: [%]
└─ Underperforming Pages (< 10 sessions):
    1. [URL] - Sessions: [#]
    2. [URL] - Sessions: [#]

CONVERSIONS
├─ Organic Form Submissions: [Number]
├─ Organic Registrations: [Number]
├─ Conv. Rate from Organic: [%]
└─ Est. Value: [Number] leads × $[Cost per lead]

TECHNICAL SEO
├─ Page Speed Score: [/100] (Target: 85+)
├─ Core Web Vitals Pass: Yes/No
├─ Crawl Errors: [Number] (Target: 0)
├─ Security Issues: [Number] (Target: 0)
└─ Mobile Usability: OK/Issues

DOMAIN AUTHORITY
├─ Domain Authority: [Score] (Target: +1-2/month)
├─ Backlinks: [Number] (Target: +5-10/month)
└─ Referring Domains: [Number]

NOTES & ACTIONS
├─ Positive: [What's working?]
├─ Issues: [What needs fixing?]
└─ Next Month Plan:
    - [ ] Action 1
    - [ ] Action 2
    - [ ] Action 3
```

---

### 2. Keyword Ranking Tracker

| Keyword | Search Volume | Month 1 Pos | Month 2 Pos | Month 3 Pos | Month 6 Pos | Target |
|---------|---------------|------------|------------|------------|------------|--------|
| บ้านเดี่ยว กรุงเทพ | 1000/mo | - | 25 | 18 | 8 | Top 5 |
| ทาวน์โฮม หทัยราษฎร์ | 500/mo | - | 32 | 20 | 7 | Top 5 |
| เดอะริคโค้ เรสซิเดนซ์ | 300/mo | - | 12 | 5 | 2 | Top 3 |
| บ้านใหม่ วงแหวน | 400/mo | - | 28 | 15 | 9 | Top 10 |
| ซื้อบ้าน จตุโชติ | 600/mo | - | 35 | 22 | 10 | Top 10 |
| ทาวน์โฮม บางแค | 200/mo | - | 40 | 25 | 12 | Top 15 |
| บ้านราคาดี กรุงเทพ | 800/mo | - | 38 | 28 | 15 | Top 10 |

**How to Fill:**
- Search volume: Use Google Keyword Planner
- Positions: Check Google Search Console Performance tab or SEMrush
- Track monthly on the same date

---

### 3. Traffic & Conversion Tracker

| Metric | Month 1 | Month 2 | Month 3 | Month 6 | Target |
|--------|---------|---------|---------|---------|--------|
| Organic Sessions | 0 | 50 | 150 | 500 | 2000+ |
| Organic Users | 0 | 40 | 120 | 400 | 1500+ |
| Bounce Rate | - | 65% | 58% | 45% | <50% |
| Avg Session Duration | - | 1:30 | 2:15 | 3:45 | >3 min |
| Form Submissions | 0 | 1 | 5 | 20 | 50+ |
| Registrations | 0 | 0 | 2 | 8 | 30+ |
| Conv. Rate | - | 2% | 3.3% | 4% | 5%+ |
| Est. Lead Value | $0 | $100 | $500 | $2000 | $5000+ |

**How to Fill:**
- Pull data from Google Analytics → Acquisition → Organic Search
- Form submissions from Google Analytics → Conversions → Goals
- Calculate Conv. Rate: (Conversions / Sessions) × 100

---

### 4. Page Performance Report

| Page URL | Sessions | Users | Bounce Rate | Avg Duration | Conv | CTR | Status |
|----------|----------|-------|-------------|--------------|------|-----|--------|
| /Project/ricco-residence-hathairat | 45 | 38 | 42% | 2:30 | 3 | 6.7% | ✓ Good |
| /Project/ricco-town-saimai | 28 | 24 | 58% | 1:45 | 1 | 3.6% | ⚠ Improve |
| / (Homepage) | 15 | 14 | 72% | 0:45 | 1 | 6.7% | ⚠ Improve |
| /About | 8 | 7 | 80% | 0:30 | 0 | - | ❌ Poor |
| /Contact | 5 | 5 | 85% | 0:20 | 0 | - | ❌ Poor |

**How to Use:**
- Pages with low sessions = low search visibility (improve keywords)
- Pages with high bounce rate = poor content/relevance (improve content)
- Pages with no conversions = improve CTA or relevance

---

### 5. Search Console Status Check

| Check | Current | Status | Action |
|-------|---------|--------|--------|
| Total Indexed URLs | 30+ | ✓ | Monitor weekly |
| Crawl Errors | 0 | ✓ | Fix immediately if > 0 |
| Mobile Usability | 0 issues | ✓ | Monitor for changes |
| Security Issues | 0 issues | ✓ | Monitor for changes |
| Coverage Issues | 0 | ✓ | Fix immediately if > 0 |
| Excluded URLs | < 5 | ✓ | Review if > 10 |
| Sitemap Status | Processed | ✓ | Resubmit monthly |

**How to Check:**
- Go to Google Search Console → Coverage tab
- All statuses should show green ✓

---

### 6. Core Web Vitals Tracking

| Date | LCP (Sec) | FID (ms) | CLS | PageSpeed Score | Status |
|------|-----------|----------|-----|-----------------|--------|
| Baseline | 3.2 | 150 | 0.15 | 72 | ❌ Below target |
| Month 1 | 2.8 | 120 | 0.12 | 78 | ⚠ Improving |
| Month 2 | 2.4 | 95 | 0.10 | 82 | ✓ Target reached |
| Month 3 | 2.2 | 85 | 0.08 | 85 | ✓ Excellent |

**Targets:**
- LCP: < 2.5 seconds
- FID: < 100 ms
- CLS: < 0.1
- PageSpeed Score: > 85/100

**Check:** https://pagespeed.web.dev/

---

### 7. Monthly Checklist

#### Week 1: Data Collection
- [ ] Pull Search Console data (impressions, clicks, positions)
- [ ] Pull Analytics data (sessions, users, conversions)
- [ ] Check PageSpeed Insights
- [ ] Check Search Console coverage

#### Week 2: Analysis
- [ ] Identify top performing keywords
- [ ] Identify pages needing improvement
- [ ] Note any penalties or issues
- [ ] Calculate trends

#### Week 3: Optimization
- [ ] Improve underperforming pages
- [ ] Update meta titles/descriptions if needed
- [ ] Create new content for gaps
- [ ] Build internal links

#### Week 4: Reporting
- [ ] Write summary (what improved, what didn't)
- [ ] Set goals for next month
- [ ] Plan content calendar
- [ ] Share results with team

---

### 8. Monthly Email Report Template

```
Subject: SEO Performance Report - [MONTH]

Hi Team,

Here's our SEO progress for [MONTH]:

🎯 KEY METRICS
- Organic Traffic: [#] sessions (+[%] vs last month)
- Keyword Rankings: [#] keywords in top 10 (+[#] this month)
- Organic Conversions: [#] leads (+[#] this month)
- Est. Value: $[Amount] (vs $[X] last month)

✅ WHAT'S WORKING
1. [Best performing keyword]
2. [Best performing page]
3. [What improved most]

⚠️ WHAT NEEDS IMPROVEMENT
1. [Page with high bounce rate]
2. [Keyword not ranking yet]
3. [Technical issue]

📋 NEXT MONTH'S PLAN
- Optimize [Page] for [Keyword]
- Create content about [Topic]
- Fix [Technical issue]
- Build backlinks to [Page]

Questions? Check the full SEO dashboard.

Best,
SEO Team
```

---

### 9. Quarterly Review

**Every 3 months, compare:**

| Metric | Q1 | Q2 | Q3 | Q4 | Growth |
|--------|-----|-----|-----|-----|--------|
| Avg Organic Sessions | 150 | 500 | 1200 | 2500 | +1667% |
| Indexed URLs | 30 | 32 | 35 | 40 | +33% |
| Keywords in Top 10 | 2 | 8 | 18 | 35 | +1650% |
| Organic Conversions | 5 | 20 | 50 | 120 | +2300% |
| Domain Authority | 5 | 10 | 18 | 25 | +400% |

---

### 10. Annual ROI Calculation

```
ORGANIC SEARCH ROI ANALYSIS - Year 1

INVESTMENT:
- Tool subscriptions: $0 (using free tools)
- Content creation time: 100 hours @ $50/hr = $5,000
- Technical optimization: 20 hours @ $75/hr = $1,500
- Total Investment: $6,500

RESULTS (Year 1):
- Total Organic Leads: 600
- Average Lead Value: $300 (avg project interest)
- Revenue Generated: 600 × $300 = $180,000
- Cost Per Lead: $6,500 / 600 = $10.83 (EXTREMELY LOW)

ROI:
- Gross ROI: ($180,000 - $6,500) / $6,500 = 2,662%
- Monthly Revenue: $15,000 from organic search alone

COMPARISON:
- Paid Ads: Cost $5-10 per click, 2% conversion = $250-500 per lead
- Organic Search: Cost $11 per lead (spreads across year)
- ORGANIC IS 20-50X CHEAPER!

Year 2+:
- Maintenance: 10 hours/month = 120 hours/year = $6,000
- Revenue: $300,000+ (compounding effect)
- Net Profit: $294,000+
```

---

## How to Use This Template

1. **Copy to Google Sheets:** https://docs.google.com/spreadsheets/
2. **Monthly Routine:**
   - First of month: Pull data from tools
   - Week 2: Analyze trends
   - Week 3: Make optimizations
   - Week 4: Report results
3. **Share with Team:** Let everyone see progress
4. **Adjust Strategy:** If something isn't working, change it

---

## Most Important Metrics (Watch These!)

### If you can only track 5 things:
1. **Impressions in Search Console** (Are people searching for you?)
2. **Clicks in Search Console** (Are people clicking your link?)
3. **Organic Sessions in Analytics** (Is traffic growing?)
4. **Conversion Rate** (Are visitors becoming leads?)
5. **Average Ranking Position** (Are you moving up in rankings?)

---

## Red Flags (Act Immediately!)

- Impressions suddenly drop → Check for Google penalty
- Indexed pages decrease → Check robots.txt, check if pages deleted
- Zero organic traffic → Check if GA4 tracking code installed
- Conversion rate < 1% → Content not matching keywords
- Core Web Vitals failing → Fix technical issues ASAP

---

**Remember: You don't improve what you don't measure!**

Check this dashboard weekly for the first month, then monthly after that.
