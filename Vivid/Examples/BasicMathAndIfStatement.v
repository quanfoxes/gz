f(a, b) {
	x = a + a - 1 * b * 7
	y = a - a * x * b
	z = x * y
	
	if z > 0 {
		=> x + y + z
	}
	else if z < 0 {
		=> x * 2
	}

	=> a + b
}

init() {
	a = 3
	b = f(1 + a, 2 * a + f(a, (a - 1) * (a + 1)))
}
