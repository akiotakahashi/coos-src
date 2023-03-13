package coos.test;

public class TestJ {

	public static int static_field;
	private int private_field;
	public int public_field;

	static void main(String[] args) {
		System.out.println("Hello, world.");
	}

	public int f() {
		return private_field+public_field;
	}

}
