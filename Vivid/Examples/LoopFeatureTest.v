import large_function()

length_of(text: link) {
	loop (i = 0, text[i] == 0, ++i) {}
}

loop_feature_test(count) {
	result = 0

	loop (i = 0, i < count, ++i) {
		++result
		large_function()
	}

	length_of('Hello')

	=> result
}

init() {
	loop_feature_test(10)
}