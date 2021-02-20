using AnKuchen.KuchenList;
using AnKuchen.Map;
using UnityEngine.UI;

namespace AbcConsole.Internal
{
    public class AbcConsoleUiElements
    {
        public InputField InputField { get; private set; }
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
            InputField = mapper.Get<InputField>("Input/InputField");
        }

        public class LogLineUiElements : IReusableMappedObject
        {
            public IMapper Mapper { get; private set; }
            public Button Button { get; private set; }
            public Text Text { get; private set; }

            public void Initialize(IMapper mapper)
            {
                Mapper = mapper;
                Button = mapper.Get<Button>();
                Text = mapper.Get<Text>("Text");
            }

            public void Activate()
            {
            }

            public void Deactivate()
            {
                Button.onClick.RemoveAllListeners();
            }
        }
    }
}