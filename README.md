# Reflection

자바의 MyBatis의 기능의 일부인 SELECT, INSERT, UPDATE, DELETE 를 C# 으로 구현해 보았습니다.

## SELECT

selectOne, selectList 호출시 객체의 변수명과 동일한 컬럼의 내용이 담겨집니다.

```
Product product = SQL.selectOne("SELECT * FROM TABLE");

List<Product> list = SQL.selectList<Product>("SELECT * FROM product");
```

## INSERT

insert 메소드 호출시 객체나, 리스트와 쿼리를 담아 호출하면 템플릿문자열($[value])를 변수의 내용과 치환하여 실행합니다.
반환값으로 마지막의 등록된 기본키의 숫자를 반환합니다.

```
Product product = new Product();
product.name = "상품";
product.price = 1000;
int rowNumber = SQL.insert(product, "INSERT INFO product (name, price) VALUE ($[name], $[price]));

List <Product> list = new List<Product>();
list.Add(product);
int rowNumber = SQL.insert(list, "INSERT INFO product (name, price) VALUE ($[name], $[price]));
```

## UPDATE

UPDATE 메소드 호출시 객체나, 리스트와 쿼리를 담아 호출하면 템플릿문자열($[value])를 변수의 내용과 치환하여 실행합니다.
반환값으로 적용된 행의 갯수를 반환합니다.

```
Product product = new Product();
product.id = "1";
product.name = "상품2";
int count = SQL.update(product, "UPDATE product SET name=$[name] WHERE id=$[id]");

List <Product> list = new List<Product>();
list.Add(product);
int count = SQL.update(list, "UPDATE product SET name=$[name] WHERE id=$[id]");
```

## DELETE

DELETE 메소드 호출시 객체나, 리스트와 쿼리를 담아 호출하면 템플릿문자열($[value])를 변수의 내용과 치환하여 실행합니다. 또한 단순 쿼리문만 으로도 호출할 수 있습니다.
반환값으로 적용된 행의 갯수를 반환합니다.

```
int count = SQL.delete("DELETE FROM product WHERE id=1)

Product product = new Product();
product.id = "1";
int count = SQL.update(product, "DELETE FROM product WHERE id=$[id]");

List <Product> list = new List<Product>();
list.Add(product);
int count = SQL.update(list, "DELETE FROM product WHERE id=$[id]");
```
