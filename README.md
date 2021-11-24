# Reflection

## 소개
 - Java의 Mybatis 처럼 쿼리 조회시 결과를 객체에 담아주거나, 객체와 쿼리문으로 메소드 호출시 내용을 치환하여 실행되도록 만들었습니다.

## 도구 및 기술

- `C#` `Reflection` `Generic`

## 작업 내용

- 제네릭을 이용하여 전달받은 객체의 종류의 타입으로 객체를 생성하여 쿼리 조회결과와 동일한 이름의 결과값을 저장 해주었습니다.
- 오버로딩을 이용하여 같은 이름의 메소드의 매개변수를 다르게 정의하여 사용하기 편의하도록 하였습니다.
- 템플릿 리터럴 기능을 구현하였습니다.

## Docs

### SELECT

selectOne, selectList 호출시 객체의 변수명과 동일한 컬럼의 내용이 담겨집니다.

```
Product product = SQL.selectOne("SELECT * FROM TABLE");

List<Product> list = SQL.selectList<Product>("SELECT * FROM product");
```

### INSERT

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

### UPDATE

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

### DELETE

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
