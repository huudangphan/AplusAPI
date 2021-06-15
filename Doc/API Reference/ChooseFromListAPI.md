# List object type: [object_type](ObjectType.md)

# List sub type: [sub_type](ObjectType.md)


# 01 Choose From List 

**Remarks**

**URL** : `/ChooseFromList/Get`

**Method** : `POST`

**Body** :

```json
{
  "object_type": 10007,
  "form_id": "10007",
  "item_id": "apz_ocrg",
  "sub_type": 0,
  "language_code": "en-US",
  "user_id": 1
}
```

```text
form_id: object_type
item_id: tên bảng
sub_type: sub object type của object_type

language_code: default là en-US, (not null)
user_id: nếu để trống sẽ sử dụng user default là -1
```

**Auth required** : `BearToken`

**Permissions required** : `None`

## Response

```json
{
  "msg_code": 200,
  "content": [
    {
      "code": "C1",
      "name": "USD",
      "locked": "Y",
      "round_sys": "Y",
      "decimals": null,
      "status": null
    },
    {
      "code": "C3",
      "name": "UR",
      "locked": "Y",
      "round_sys": "Y",
      "decimals": null,
      "status": null
    },
    {
      "code": "C2",
      "name": "VND",
      "locked": "Y",
      "round_sys": "N",
      "decimals": null,
      "status": null
    }
  ],
  "message": null
}
```

