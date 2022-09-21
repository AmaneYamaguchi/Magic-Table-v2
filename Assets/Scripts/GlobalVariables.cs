public static class GlobalVariables { // グローバル変数っぽいものをここにまとめる

    public enum scenes : int { square, pentagon, triangle};//めんどかったらあとでただのintに変える
    public static scenes sceneNo = scenes.square;// シーン番号
	public static float Gain = 4f/3f;

}
