f(a, b) {
	x = 2 * a
	y = 7 + b + x

	if x > 10 {
		x += y
		y *= 10 + x
	}
	else {
		x -= y
		y /= 10 + x
	}

	=> x + a + y + b
}

init() {
	f(1, 2)
}