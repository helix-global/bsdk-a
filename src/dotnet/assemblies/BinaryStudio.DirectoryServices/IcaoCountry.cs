﻿using System;
using System.Collections.Generic;

namespace BinaryStudio.DirectoryServices
    {
    public class IcaoCountry
        {
        public String TwoLetterISOCountryName { get; }
        public String ThreeLetterISOCountryName { get; }
        public String EnglishShortName { get; }
        public String RussianShortName { get; }

        protected internal IcaoCountry(String Eshortname, String Rshortname, String two, String three) {
            EnglishShortName = Eshortname;
            RussianShortName = Rshortname;
            TwoLetterISOCountryName = two;
            ThreeLetterISOCountryName = three;
            }

        public static readonly IDictionary<String, IcaoCountry> TwoLetterCountries = new SortedDictionary<String, IcaoCountry> {
            {"af",new IcaoCountry("Afghanistan","Афганистан","af","afg")},
            {"al",new IcaoCountry("Albania","Албания","al","alb")},
            {"aq",new IcaoCountry("Antarctica","Антарктика","aq","ata")},
            {"dz",new IcaoCountry("Algeria","Алжир","dz","dza")},
            {"as",new IcaoCountry("American Samoa","Американское Самоа","as","asm")},
            {"ad",new IcaoCountry("Andorra","Андорра","ad","and")},
            {"ao",new IcaoCountry("Angola","Ангола","ao","ago")},
            {"ag",new IcaoCountry("Antigua and Barbuda","Антигуа и Барбуда","ag","atg")},
            {"az",new IcaoCountry("Azerbaijan","Азербайджан","az","aze")},
            {"ar",new IcaoCountry("Argentina","Аргентина","ar","arg")},
            {"au",new IcaoCountry("Australia","Австралия","au","aus")},
            {"at",new IcaoCountry("Austria","Австрия","at","aut")},
            {"bs",new IcaoCountry("Bahamas","Багамские Острова","bs","bhs")},
            {"bh",new IcaoCountry("Bahrain","Бахрейн","bh","bhr")},
            {"bd",new IcaoCountry("Bangladesh","Бангладеш","bd","bgd")},
            {"am",new IcaoCountry("Armenia","Армения","am","arm")},
            {"bb",new IcaoCountry("Barbados","Барбадос","bb","brb")},
            {"be",new IcaoCountry("Belgium","Бельгия","be","bel")},
            {"bm",new IcaoCountry("Bermuda","Бермуды","bm","bmu")},
            {"bt",new IcaoCountry("Bhutan","Бутан","bt","btn")},
            {"bo",new IcaoCountry("Bolivia","Боливия","bo","bol")},
            {"ba",new IcaoCountry("Bosnia and Herzegovina","Босния и Герцеговина","ba","bih")},
            {"bw",new IcaoCountry("Botswana","Ботсвана","bw","bwa")},
            {"bv",new IcaoCountry("Bouvet Island","Остров Буве","bv","bvt")},
            {"br",new IcaoCountry("Brazil","Бразилия","br","bra")},
            {"bz",new IcaoCountry("Belize","Белиз","bz","blz")},
            {"io",new IcaoCountry("British Indian Ocean Territory","Британская территория в Индийском океане","io","iot")},
            {"sb",new IcaoCountry("Solomon Islands","Соломоновы Острова","sb","slb")},
            {"vg",new IcaoCountry("Virgin Islands (British)","Виргинские Острова (Великобритания)","vg","vgb")},
            {"bn",new IcaoCountry("Brunei Darussalam","Бруней","bn","brn")},
            {"bg",new IcaoCountry("Bulgaria","Болгария","bg","bgr")},
            {"mm",new IcaoCountry("Myanmar","Мьянма","mm","mmr")},
            {"bi",new IcaoCountry("Burundi","Бурунди","bi","bdi")},
            {"by",new IcaoCountry("Belarus","Белоруссия","by","blr")},
            {"kh",new IcaoCountry("Cambodia","Камбоджа","kh","khm")},
            {"cm",new IcaoCountry("Cameroon","Камерун","cm","cmr")},
            {"ca",new IcaoCountry("Canada","Канада","ca","can")},
            {"cv",new IcaoCountry("Cabo Verde","Кабо-Верде","cv","cpv")},
            {"ky",new IcaoCountry("Cayman Islands","Острова Кайман","ky","cym")},
            {"cf",new IcaoCountry("Central African Republic","ЦАР","cf","caf")},
            {"lk",new IcaoCountry("Sri Lanka","Шри-Ланка","lk","lka")},
            {"td",new IcaoCountry("Chad","Чад","td","tcd")},
            {"cl",new IcaoCountry("Chile","Чили","cl","chl")},
            {"cn",new IcaoCountry("China","Китай","cn","chn")},
            {"tw",new IcaoCountry("Taiwan","Тайвань","tw","twn")},
            {"cx",new IcaoCountry("Christmas Island","Остров Рождества","cx","cxr")},
            {"cc",new IcaoCountry("Cocos (Keeling Islands)","Кокосовые острова","cc","cck")},
            {"co",new IcaoCountry("Colombia","Колумбия","co","col")},
            {"km",new IcaoCountry("Comoros","Коморы","km","com")},
            {"yt",new IcaoCountry("Mayotte","Майотта","yt","myt")},
            {"cg",new IcaoCountry("Congo","Республика Конго","cg","cog")},
            {"cd",new IcaoCountry("Democratic Republic of the Congo","Демократическая Республика Конго","cd","cod")},
            {"ck",new IcaoCountry("Cook Islands","Острова Кука","ck","cok")},
            {"cr",new IcaoCountry("Costa Rica","Коста-Рика","cr","cri")},
            {"hr",new IcaoCountry("Croatia","Хорватия","hr","hrv")},
            {"cu",new IcaoCountry("Cuba","Куба","cu","cub")},
            {"cy",new IcaoCountry("Cyprus","Кипр","cy","cyp")},
            {"cz",new IcaoCountry("Czechia","Чехия","cz","cze")},
            {"bj",new IcaoCountry("Benin","Бенин","bj","ben")},
            {"dk",new IcaoCountry("Denmark","Дания","dk","dnk")},
            {"dm",new IcaoCountry("Dominica","Доминика","dm","dma")},
            {"do",new IcaoCountry("Dominican Republic","Доминиканская Республика","do","dom")},
            {"ec",new IcaoCountry("Ecuador","Эквадор","ec","ecu")},
            {"sv",new IcaoCountry("El Salvador","Сальвадор","sv","slv")},
            {"gq",new IcaoCountry("Equatorial Guinea","Экваториальная Гвинея","gq","gnq")},
            {"et",new IcaoCountry("Ethiopia","Эфиопия","et","eth")},
            {"er",new IcaoCountry("Eritrea","Эритрея","er","eri")},
            {"ee",new IcaoCountry("Estonia","Эстония","ee","est")},
            {"fo",new IcaoCountry("Faroe Islands","Фарерские острова","fo","fro")},
            {"fk",new IcaoCountry("Falkland Islands [Malvinas]","Фолклендские острова","fk","flk")},
            {"gs",new IcaoCountry("South Georgia and the South Sandwich Islands","Южная Георгия и Южные Сандвичевы Острова","gs","sgs")},
            {"fj",new IcaoCountry("Fiji","Фиджи","fj","fji")},
            {"fi",new IcaoCountry("Finland","Финляндия","fi","fin")},
            {"ax",new IcaoCountry("Åland Islands","Аландские острова","ax","ala")},
            {"fr",new IcaoCountry("France","Франция","fr","fra")},
            {"gf",new IcaoCountry("French Guiana","Гвиана","gf","guf")},
            {"pf",new IcaoCountry("French Polynesia","Французская Полинезия","pf","pyf")},
            {"tf",new IcaoCountry("French Southern Territories","Французские Южные и Антарктические территории","tf","atf")},
            {"dj",new IcaoCountry("Djibouti","Джибути","dj","dji")},
            {"ga",new IcaoCountry("Gabon","Габон","ga","gab")},
            {"ge",new IcaoCountry("Georgia","Грузия","ge","geo")},
            {"gm",new IcaoCountry("Gambia","Гамбия","gm","gmb")},
            {"ps",new IcaoCountry("Palestine, State of","Государство Палестина","ps","pse")},
            {"de",new IcaoCountry("Germany","Германия","de","deu")},
            {"gh",new IcaoCountry("Ghana","Гана","gh","gha")},
            {"gi",new IcaoCountry("Gibraltar","Гибралтар","gi","gib")},
            {"ki",new IcaoCountry("Kiribati","Кирибати","ki","kir")},
            {"gr",new IcaoCountry("Greece","Греция","gr","grc")},
            {"gl",new IcaoCountry("Greenland","Гренландия","gl","grl")},
            {"gd",new IcaoCountry("Grenada","Гренада","gd","grd")},
            {"gp",new IcaoCountry("Guadeloupe","Гваделупа","gp","glp")},
            {"gu",new IcaoCountry("Guam","Гуам","gu","gum")},
            {"gt",new IcaoCountry("Guatemala","Гватемала","gt","gtm")},
            {"gn",new IcaoCountry("Guinea","Гвинея","gn","gin")},
            {"gy",new IcaoCountry("Guyana","Гайана","gy","guy")},
            {"ht",new IcaoCountry("Haiti","Гаити","ht","hti")},
            {"hm",new IcaoCountry("Heard Island and McDonald Islands","Херд и Макдональд","hm","hmd")},
            {"va",new IcaoCountry("Holy See","Ватикан","va","vat")},
            {"hn",new IcaoCountry("Honduras","Гондурас","hn","hnd")},
            {"hk",new IcaoCountry("Hong Kong","Гонконг","hk","hkg")},
            {"hu",new IcaoCountry("Hungary","Венгрия","hu","hun")},
            {"is",new IcaoCountry("Iceland","Исландия","is","isl")},
            {"in",new IcaoCountry("India","Индия","in","ind")},
            {"id",new IcaoCountry("Indonesia","Индонезия","id","idn")},
            {"ir",new IcaoCountry("Iran","Иран","ir","irn")},
            {"iq",new IcaoCountry("Iraq","Ирак","iq","irq")},
            {"ie",new IcaoCountry("Ireland","Ирландия","ie","irl")},
            {"il",new IcaoCountry("Israel","Израиль","il","isr")},
            {"it",new IcaoCountry("Italy","Италия","it","ita")},
            {"ci",new IcaoCountry("Côte d'Ivoire","Кот-д’Ивуар","ci","civ")},
            {"jm",new IcaoCountry("Jamaica","Ямайка","jm","jam")},
            {"jp",new IcaoCountry("Japan","Япония","jp","jpn")},
            {"kz",new IcaoCountry("Kazakhstan","Казахстан","kz","kaz")},
            {"jo",new IcaoCountry("Jordan","Иордания","jo","jor")},
            {"ke",new IcaoCountry("Kenya","Кения","ke","ken")},
            {"kp",new IcaoCountry("Democratic People's Republic of Korea","КНДР","kp","prk")},
            {"kr",new IcaoCountry("Republic of Korea","Республика Корея","kr","kor")},
            {"kw",new IcaoCountry("Kuwait","Кувейт","kw","kwt")},
            {"kg",new IcaoCountry("Kyrgyzstan","Киргизия","kg","kgz")},
            {"la",new IcaoCountry("Lao People's Democratic Republic","Лаос","la","lao")},
            {"lb",new IcaoCountry("Lebanon","Ливан","lb","lbn")},
            {"ls",new IcaoCountry("Lesotho","Лесото","ls","lso")},
            {"lv",new IcaoCountry("Latvia","Латвия","lv","lva")},
            {"lr",new IcaoCountry("Liberia","Либерия","lr","lbr")},
            {"ly",new IcaoCountry("Libya","Ливия","ly","lby")},
            {"li",new IcaoCountry("Liechtenstein","Лихтенштейн","li","lie")},
            {"lt",new IcaoCountry("Lithuania","Литва","lt","ltu")},
            {"lu",new IcaoCountry("Luxembourg","Люксембург","lu","lux")},
            {"mo",new IcaoCountry("Macao","Макао","mo","mac")},
            {"mg",new IcaoCountry("Madagascar","Мадагаскар","mg","mdg")},
            {"mw",new IcaoCountry("Malawi","Малави","mw","mwi")},
            {"my",new IcaoCountry("Malaysia","Малайзия","my","mys")},
            {"mv",new IcaoCountry("Maldives","Мальдивы","mv","mdv")},
            {"ml",new IcaoCountry("Mali","Мали","ml","mli")},
            {"mt",new IcaoCountry("Malta","Мальта","mt","mlt")},
            {"mq",new IcaoCountry("Martinique","Мартиника","mq","mtq")},
            {"mr",new IcaoCountry("Mauritania","Мавритания","mr","mrt")},
            {"mu",new IcaoCountry("Mauritius","Маврикий","mu","mus")},
            {"mx",new IcaoCountry("Mexico","Мексика","mx","mex")},
            {"mc",new IcaoCountry("Monaco","Монако","mc","mco")},
            {"mn",new IcaoCountry("Mongolia","Монголия","mn","mng")},
            {"md",new IcaoCountry("Moldova","Молдавия","md","mda")},
            {"me",new IcaoCountry("Montenegro","Черногория","me","mne")},
            {"ms",new IcaoCountry("Montserrat","Монтсеррат","ms","msr")},
            {"ma",new IcaoCountry("Morocco","Марокко","ma","mar")},
            {"mz",new IcaoCountry("Mozambique","Мозамбик","mz","moz")},
            {"om",new IcaoCountry("Oman","Оман","om","omn")},
            {"na",new IcaoCountry("Namibia","Намибия","na","nam")},
            {"nr",new IcaoCountry("Nauru","Науру","nr","nru")},
            {"np",new IcaoCountry("Nepal","Непал","np","npl")},
            {"nl",new IcaoCountry("Netherlands","Нидерланды","nl","nld")},
            {"cw",new IcaoCountry("Curaçao","Кюрасао","cw","cuw")},
            {"aw",new IcaoCountry("Aruba","Аруба","aw","abw")},
            {"sx",new IcaoCountry("Sint Maarten (Dutch part)","Синт-Мартен","sx","sxm")},
            {"bq",new IcaoCountry("Bonaire, Sint Eustatius and Saba","Бонайре","bq","bes")},
            {"nc",new IcaoCountry("New Caledonia","Новая Каледония","nc","ncl")},
            {"vu",new IcaoCountry("Vanuatu","Вануату","vu","vut")},
            {"nz",new IcaoCountry("New Zealand","Новая Зеландия","nz","nzl")},
            {"ni",new IcaoCountry("Nicaragua","Никарагуа","ni","nic")},
            {"ne",new IcaoCountry("Niger","Нигер","ne","ner")},
            {"ng",new IcaoCountry("Nigeria","Нигерия","ng","nga")},
            {"nu",new IcaoCountry("Niue","Ниуэ","nu","niu")},
            {"nf",new IcaoCountry("Norfolk Island","Остров Норфолк","nf","nfk")},
            {"no",new IcaoCountry("Norway","Норвегия","no","nor")},
            {"mp",new IcaoCountry("Northern Mariana Islands","Северные Марианские Острова","mp","mnp")},
            {"um",new IcaoCountry("United States Minor Outlying Islands","Внешние малые острова США","um","umi")},
            {"fm",new IcaoCountry("Micronesia","Микронезия","fm","fsm")},
            {"mh",new IcaoCountry("Marshall Islands","Маршалловы Острова","mh","mhl")},
            {"pw",new IcaoCountry("Palau","Палау","pw","plw")},
            {"pk",new IcaoCountry("Pakistan","Пакистан","pk","pak")},
            {"pa",new IcaoCountry("Panama","Панама","pa","pan")},
            {"pg",new IcaoCountry("Papua New Guinea","Папуа - Новая Гвинея","pg","png")},
            {"py",new IcaoCountry("Paraguay","Парагвай","py","pry")},
            {"pe",new IcaoCountry("Peru","Перу","pe","per")},
            {"ph",new IcaoCountry("Philippines","Филиппины","ph","phl")},
            {"pn",new IcaoCountry("Pitcairn","Острова Питкэрн","pn","pcn")},
            {"pl",new IcaoCountry("Poland","Польша","pl","pol")},
            {"pt",new IcaoCountry("Portugal","Португалия","pt","prt")},
            {"gw",new IcaoCountry("Guinea-Bissau","Гвинея-Бисау","gw","gnb")},
            {"tl",new IcaoCountry("Timor-Leste","Восточный Тимор","tl","tls")},
            {"pr",new IcaoCountry("Puerto Rico","Пуэрто-Рико","pr","pri")},
            {"qa",new IcaoCountry("Qatar","Катар","qa","qat")},
            {"re",new IcaoCountry("Réunion","Реюньон","re","reu")},
            {"ro",new IcaoCountry("Romania","Румыния","ro","rou")},
            {"ru",new IcaoCountry("Russian Federation","Россия","ru","rus")},
            {"rw",new IcaoCountry("Rwanda","Руанда","rw","rwa")},
            {"bl",new IcaoCountry("Saint Barthélemy","Сен-Бартелеми","bl","blm")},
            {"sh",new IcaoCountry("Saint Helena, Ascension and Tristan da Cunha","Острова Святой Елены","sh","shn")},
            {"kn",new IcaoCountry("Saint Kitts and Nevis","Сент-Китс и Невис","kn","kna")},
            {"ai",new IcaoCountry("Anguilla","Ангилья","ai","aia")},
            {"lc",new IcaoCountry("Saint Lucia","Сент-Люсия","lc","lca")},
            {"mf",new IcaoCountry("Saint Martin (French part)","Сен-Мартен","mf","maf")},
            {"pm",new IcaoCountry("Saint Pierre and Miquelon","Сен-Пьер и Микелон","pm","spm")},
            {"vc",new IcaoCountry("Saint Vincent and the Grenadines","Сент-Винсент и Гренадины","vc","vct")},
            {"sm",new IcaoCountry("San Marino","Сан-Марино","sm","smr")},
            {"st",new IcaoCountry("Sao Tome and Principe","Сан-Томе и Принсипи","st","stp")},
            {"sa",new IcaoCountry("Saudi Arabia","Саудовская Аравия","sa","sau")},
            {"sn",new IcaoCountry("Senegal","Сенегал","sn","sen")},
            {"rs",new IcaoCountry("Serbia","Сербия","rs","srb")},
            {"sc",new IcaoCountry("Seychelles","Сейшельские Острова","sc","syc")},
            {"sl",new IcaoCountry("Sierra Leone","Сьерра-Леоне","sl","sle")},
            {"sg",new IcaoCountry("Singapore","Сингапур","sg","sgp")},
            {"sk",new IcaoCountry("Slovakia","Словакия","sk","svk")},
            {"vn",new IcaoCountry("Viet Nam","Вьетнам","vn","vnm")},
            {"si",new IcaoCountry("Slovenia","Словения","si","svn")},
            {"so",new IcaoCountry("Somalia","Сомали","so","som")},
            {"za",new IcaoCountry("South Africa","ЮАР","za","zaf")},
            {"zw",new IcaoCountry("Zimbabwe","Зимбабве","zw","zwe")},
            {"es",new IcaoCountry("Spain","Испания","es","esp")},
            {"ss",new IcaoCountry("South Sudan","Южный Судан","ss","ssd")},
            {"sd",new IcaoCountry("Sudan","Судан","sd","sdn")},
            {"eh",new IcaoCountry("Western Sahara","САДР","eh","esh")},
            {"sr",new IcaoCountry("Suriname","Суринам","sr","sur")},
            {"sj",new IcaoCountry("Svalbard and Jan Mayen","Шпицберген и Ян-Майен","sj","sjm")},
            {"sz",new IcaoCountry("Eswatini","Эсватини","sz","swz")},
            {"se",new IcaoCountry("Sweden","Швеция","se","swe")},
            {"ch",new IcaoCountry("Switzerland","Швейцария","ch","che")},
            {"sy",new IcaoCountry("Syrian Arab Republic","Сирия","sy","syr")},
            {"tj",new IcaoCountry("Tajikistan","Таджикистан","tj","tjk")},
            {"th",new IcaoCountry("Thailand","Таиланд","th","tha")},
            {"tg",new IcaoCountry("Togo","Того","tg","tgo")},
            {"tk",new IcaoCountry("Tokelau","Токелау","tk","tkl")},
            {"to",new IcaoCountry("Tonga","Тонга","to","ton")},
            {"tt",new IcaoCountry("Trinidad and Tobago","Тринидад и Тобаго","tt","tto")},
            {"ae",new IcaoCountry("United Arab Emirates","ОАЭ","ae","are")},
            {"tn",new IcaoCountry("Tunisia","Тунис","tn","tun")},
            {"tr",new IcaoCountry("Turkey","Турция","tr","tur")},
            {"tm",new IcaoCountry("Turkmenistan","Туркмения","tm","tkm")},
            {"tc",new IcaoCountry("Turks and Caicos Islands","Теркс и Кайкос","tc","tca")},
            {"tv",new IcaoCountry("Tuvalu","Тувалу","tv","tuv")},
            {"ug",new IcaoCountry("Uganda","Уганда","ug","uga")},
            {"ua",new IcaoCountry("Ukraine","Украина","ua","ukr")},
            {"mk",new IcaoCountry("North Macedonia","Северная Македония","mk","mkd")},
            {"eg",new IcaoCountry("Egypt","Египет","eg","egy")},
            {"gb",new IcaoCountry("United Kingdom of Great Britain and Northern Ireland","Великобритания","gb","gbr")},
            {"gg",new IcaoCountry("Guernsey","Гернси","gg","ggy")},
            {"je",new IcaoCountry("Jersey","Джерси","je","jey")},
            {"im",new IcaoCountry("Isle of Man","Остров Мэн","im","imn")},
            {"tz",new IcaoCountry("Tanzania","Танзания","tz","tza")},
            {"us",new IcaoCountry("United States of America","США","us","usa")},
            {"vi",new IcaoCountry("Virgin Islands (U.S.)","Виргинские Острова (США)","vi","vir")},
            {"bf",new IcaoCountry("Burkina Faso","Буркина-Фасо","bf","bfa")},
            {"uy",new IcaoCountry("Uruguay","Уругвай","uy","ury")},
            {"uz",new IcaoCountry("Uzbekistan","Узбекистан","uz","uzb")},
            {"ve",new IcaoCountry("Venezuela","Венесуэла","ve","ven")},
            {"wf",new IcaoCountry("Wallis and Futuna","Уоллис и Футуна","wf","wlf")},
            {"ws",new IcaoCountry("Samoa","Самоа","ws","wsm")},
            {"ye",new IcaoCountry("Yemen","Йемен","ye","yem")},
            {"zm",new IcaoCountry("Zambia","Замбия","zm","zmb")},
            {"un",new IcaoCountry("United Nations","ООН","un",null)},
            {"zz",new IcaoCountry("United Nations","ООН","zz",null)},
            {"ks",new IcaoCountry("Kosovo","Республика Косово","ks","kos")},
            {"aa",new IcaoCountry("Abkhazia","Абхазия","aa","aab")},
            {"ab",new IcaoCountry("Abkhazia","Абхазия","ab","abh")},
            {"os",new IcaoCountry("South Ossetia","Южная Осетия","os","ost")},
            {"xo",new IcaoCountry("Sovereign Order Of Malta","Суверенный Военный Мальтийский Орден","xo",null)},
            {"eu",new IcaoCountry("European Union","Европейский Союз","eu",null)},
            };

        public static readonly IDictionary<String,IcaoCountry> ThreeLetterCountries;
        static IcaoCountry()
            {
            ThreeLetterCountries = new Dictionary<String, IcaoCountry>();
            foreach (var i in TwoLetterCountries) {
                if (!String.IsNullOrWhiteSpace(i.Value.ThreeLetterISOCountryName)) {
                    ThreeLetterCountries[i.Value.ThreeLetterISOCountryName] = i.Value;
                    }
                }
            ThreeLetterCountries["car"] = ThreeLetterCountries["caf"];
            }
        }
    }