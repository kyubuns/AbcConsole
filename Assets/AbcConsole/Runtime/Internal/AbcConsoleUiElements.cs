using AnKuchen.KuchenList;
using AnKuchen.Map;
using UnityEngine.UI;

namespace AbcConsole.Internal
{
    public class AbcConsoleUiElements
    {
        public VerticalList<LogLineUiElements> Log { get; private set; }

        public AbcConsoleUiElements(IMapper mapper)
        {
            Initialize(mapper);
        }

        private void Initialize(IMapper mapper)
        {
            Log = new VerticalList<LogLineUiElements>(
                mapper.Get<ScrollRect>("Log"),
                mapper.GetChild<LogLineUiElements>("LogLine")
            );
        }

        public class LogLineUiElements : IMappedObject
        {
            public IMapper Mapper { get; private set; }
            public Text Text { get; private set; }

            public void Initialize(IMapper mapper)
            {
                Mapper = mapper;
                Text = mapper.Get<Text>("Text");
            }
        }
    }
}