using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[MessagePackObject]
public class GameData
{
	/// <summary>
	/// お金
	/// </summary>
	[Key(0)]
	public ulong Money { get; set; }

    /// <summary>
    /// 食材の種類の数（随時更新）
    /// </summary>
    private const int INGREDIENTS_SIZE = 10;
	/// <summary>
	/// 食材の数（nullはまだ手に入れてないやつ）
	/// </summary>
	[Key(1)]
	public uint?[] Ingredients { get; private set; }

    /// <summary>
    /// 料理の数（随時更新）
    /// </summary>
    private const int COOKING_SIZE = 5;
	/// <summary>
	/// 料理を作ったかどうか
	/// </summary>
	[Key(2)]
	public bool[] Cooking { get; private set; }

    /// <summary>
    /// レシピの数（随時更新）
    /// </summary>
    private const int RECIPE_SIZE = 15;
	/// <summary>
	/// レシピを入手したかどうか
	/// </summary>
	[Key(3)]
	public bool[] Recipe { get; private set; }

    public GameData()
    {
        Money = 0;
		Ingredients = Enumerable.Repeat<uint?>(null, INGREDIENTS_SIZE).ToArray();
		Cooking = Enumerable.Repeat<bool>(false, COOKING_SIZE).ToArray();
        Recipe = Enumerable.Repeat<bool>(false, RECIPE_SIZE).ToArray();
    }
}


