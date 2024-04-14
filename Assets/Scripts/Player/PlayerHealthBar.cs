using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
	public int maxHealth = 100;
	public HealthBar healthBar;
	public int currentHealth;
	private bool isPlayer;
	public const int sortingOrderDefault = 5000;

	private class FunctionUpdater
	{
		private class MonoBehaviourHook : MonoBehaviour
		{
			public System.Action OnUpdate;

			private void Update()
			{
				if (OnUpdate != null) OnUpdate();
			}
		}

		private static GameObject initGameObject;
		private static void InitIfNeeded()
		{
			if (initGameObject == null)
			{
				initGameObject = new GameObject("FunctionUpdater_Global");
			}
		}

		public static FunctionUpdater Create(System.Func<bool> updateFunc, string functionName, bool active, bool stopAllWithSameName)
		{
			InitIfNeeded();

			GameObject gameObject = new GameObject("FunctionUpdater Object " + functionName, typeof(MonoBehaviourHook));
			FunctionUpdater functionUpdater = new FunctionUpdater();
			gameObject.GetComponent<MonoBehaviourHook>().OnUpdate = () =>
			{
				if (active && updateFunc())
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			};

			return functionUpdater;
		}
	}

	private void Start()
	{
		isPlayer = CompareTag("Player");
		if (isPlayer)
		{
			currentHealth = GameManager.Instance.GetPlayerHealth();
			healthBar.SetHealth(currentHealth);
		}
		else
		{
			healthBar.SetHealth(maxHealth);
		}
	}

	public static void TextPopup(string text, Vector3 position, float popupTime = 0.5f)
	{
		CreateWorldTextPopup(text, position, popupTime);
	}

	public static void CreateWorldTextPopup(string text, Vector3 localPosition, float popupTime = 0.5f)
	{
		CreateWorldTextPopup(null, text, localPosition, 5, Color.red, localPosition + new Vector3(0, 2f, 0), popupTime);
	}


	public static void CreateWorldTextPopup(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, Vector3 finalPopupPosition, float popupTime)
	{
		TextMesh textMesh = CreateWorldText(parent, text, localPosition, fontSize, color, TextAnchor.LowerLeft, TextAlignment.Left, sortingOrderDefault);
		Transform transform = textMesh.transform;
		Vector3 moveAmount = (finalPopupPosition - localPosition) / popupTime;
		FunctionUpdater.Create(() =>
		{
			transform.position += moveAmount * Time.unscaledDeltaTime;
			popupTime -= Time.unscaledDeltaTime;
			if (popupTime <= 0f)
			{
				UnityEngine.Object.Destroy(transform.gameObject);
				return true;
			}
			else
			{
				return false;
			}
		}, "WorldTextPopup", true, false);
	}

	public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 1, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = sortingOrderDefault)
	{
		if (color == null) color = Color.white;
		TextMesh textMesh = CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
		textMesh.characterSize = 0.1f;
		return textMesh;
	}

	public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
	{
		GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
		Transform transform = gameObject.transform;
		transform.SetParent(parent, false);
		transform.localPosition = localPosition;
		TextMesh textMesh = gameObject.GetComponent<TextMesh>();
		textMesh.anchor = textAnchor;
		textMesh.alignment = textAlignment;
		textMesh.text = text;
		textMesh.fontSize = fontSize;
		textMesh.color = color;
		textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
		return textMesh;
	}

	public int TakeDamage(int damage, bool isPlayer = true)
	{
		Debug.Log("Taking damage: " + damage);
		TextPopup(damage.ToString(), transform.position - new Vector3(0f, -1f, 0f));
		int oldHealth = currentHealth;
		currentHealth = Mathf.Min(Mathf.Max(0, currentHealth - damage), 100);

		if (isPlayer)
		{
			GameManager.Instance.SetPlayerHealth(currentHealth);
		}
		healthBar.SetHealth(currentHealth);

		return currentHealth;
	}

	public void HPManager(int damage)
	{
		TakeDamage(damage, true);
	}

	/*
	* This should be move to somewhere else later
	*/
	public void AddCoins(float coin)
	{
		GameManager.Instance.SetCoins(GameManager.Instance.GetCoins() + coin);
	}
}
