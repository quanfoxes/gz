CURRENCY_EUROS = 0i8
CURRENCY_DOLLARS = 1i8

Pair<A, B> {
	first: A
	second: B

	init(a: A, b: B) {
		first = a
		second = b
	}
}

Pack<A, B> {
	first: Pair<A, B>
	second: Pair<A, B>
	third: Pair<A, B>

	get(i: large) {
		if i == 0 {
			=> first
		}
		else i == 1 {
			=> second
		}
		else i == 2 {
			=> third
		}
		else {
			=> none as Pair<A, B>
		}
	}

	set(i: large, value: Pair<A, B>) {
		if i == 0 {
			first = value
		}
		else i == 1 {
			second = value
		}
		else i == 2 {
			third = value
		}
	}
}

Product {
	name: String

	enchant() {
		name = String('i') + name
	}

	is_enchanted() {
		if name[0] == 105 {
			=> true
		}

		=> false
	}
}

Price {
	value: large
	currency: tiny

	convert(c: tiny) {
		if currency == c {
			=> value
		}

		if c == CURRENCY_EUROS {
			=> value * 0.8
		}
		else {
			=> value * 1.25
		}
	}
}

export create_pack() {
	=> Pack<Product, Price>()
}

export set_product(pack: Pack<Product, Price>, i: large, name: link, value: large, currency: tiny) {
	product = Product()
	product.name = String(name)

	price = Price()
	price.value = value
	price.currency = currency

	pack[i] = Pair<Product, Price>(product, price)
}

export get_product_name(pack: Pack<Product, Price>, i: large) {
	=> pack[i].first.name
}

export enchant_product(pack: Pack<Product, Price>, i: large) {
	pack[i].first.enchant()
}

export is_product_enchanted(pack: Pack<Product, Price>, i: large) {
	=> pack[i].first.is_enchanted()
}

export get_product_price(pack: Pack<Product, Price>, i: large, currency: tiny) {
	=> pack[i].second.convert(currency)
}

init() {
	=> true

	pack = create_pack()
	set_product(pack, 0, 0 as link, 0, 0 as tiny)
	get_product_name(pack, 0)
	enchant_product(pack, 0)
	is_product_enchanted(pack, 0)
	get_product_price(pack, 0, 0i8)
}