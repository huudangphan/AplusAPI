# DOCUMENT LOGIN API

- **ApiVersion**: 1.0
- **BasePath**: http://172.16.8.60:8124
- **ResourcePath**: /User, /Token
- **Produces**:
    - application/json
- **Message code**
```
    None = 0,
    Success = 200,
    Error = 2,
    ExceptionMessage = 191
```
- **Opis**:
    # 01 API Login
    - **Path**: `/User/Login`</br>
    Operation: 
        - **Method**: `POST`
        - Note: Trả về thông tin token.
        - Authentications: Null
        - **Body**:
            ```json
            [{
                "Email":"Thienthn@apzon.com",                       Email đăng nhập
                "Password":"123456apzon"                            Mật khẩu
            }]
            ```
        - **ReponseMessage**:
            ```json
            {
                "Content": 
                {
                    "TokenId": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6WyJ0aGllbnRobkBhcHpvbi5jb20iLCI4ZTVmNWYwOS05M2NmLTRhZTgtYmNjNS01MzFmODJlY2Y2NjIiLCIyMDIwMTEwMiJdLCJuYmYiOjE2MDE2MDY5MDYsImV4cCI6MTYwNDI4NTMwNiwiaWF0IjoxNjAxNjA2OTA2fQ.fD0eUgzn2PoCetYfQ0IohBjsAvKrEHyF1FmohCR5kxU",
                    "Email": "Thienthn@apzon.com",
                    "ExpiredTime": "2020-11-02T09:38:58.2301601+07:00",
                    "Status": "R"
                },
                "Message": null,
                "MessageCode": 200
            }
            ```
            
    # 02 API Login third party
    - **Path**: `/User/LoginThirdParty?access_token=&type=`</br>
    Operation: 
        - **Method**: `GET`
        - Note: Trả về thông tin token.
        - Authentications: Null
        - **Params**: access_token (Token get từ bên thứ 3), type (1: FB, 2: Google)
        - **ReponseMessage**:
            ```json
            {
                "Content": {
                    "TokenId": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6WyJ0aGllbnRobkBhcHpvbi5jb20iLCIxMi8yLzIwMjAgMTE6NTQ6MjcgQU0iLCIyMDIxMDEwMiJdLCJuYmYiOjE2MDY4ODQ4NjcsImV4cCI6MTYwOTU2MzI2NywiaWF0IjoxNjA2ODg0ODY3fQ.ZqGWKG_-KiEJ32SJN9mrXW0B8cIB2eToPaqNNjz4638",
                    "Email": "thienthn@apzon.com",
                    "ExpiredTime": "2021-01-02T11:54:27.963",
                    "Status": "R",
                    "Type": "N"
                },
                "Message": "Success",
                "MessageCode": 200
            }
            ```
            
    # 04 API Register
    - **Path**: /User/Register</br>
    Operation: 
        - **Method**: `POST`
        - Note: Trả về thông tin tài khoản.
        - Authentications: Null
        - **Body**:
            ```json    
            [{
                "Email": "Thienthn@apzon.com",
                "Password":"123456apzon",
                "FullName":"Ngoc Thiên",
                "Phone": "0574654766",
                "Address": "999 Võ Văn Kiệt"
            }]
            ```
        - **ReponseMessage**:
            ```json
            {
                "Content": 
                {
                    "Email": "Thienthn@apzon.com",
                    "Password": "*************"
                },
                "Message": "Resgisted Successfully.",
                "MessageCode": 200
            }
            ```
    # 05 API Get Infomation User
    - **Path**: /User/GetUser?email=</br>
    Operation: 
        - **Method**: `GET`
        - Note: Trả về thông tin tài khoản.
        - **Authentications require**: `Bearer Token`
        - **Header**: Token: TokenId (Token nhận khi đăng nhập)
        - **ReponseMessage**:
            ```json
            {
                "Content": 
                {
                    "Email": "Thienthn@apzon.com",
                    "FullName": "Ngoc Thiên",
                    "EmailValidate": "Thienthn@apzon.com",
                    "Phone": "0574654766",
                    "Address": "999 Võ Văn Kiệt"
                },
                "Message": "Successful.",
                "MessageCode": 200
            }
            ``` 
    # 06 API Update User       
    - **Path**: `/User/UpdateUser`</br>
        Operation:
        - **Method**: `POST`
        - Note: Trả về thông tin user.
        - **Authentications require**: `Bearer Token`
        - **Header**: Token: TokenId (Token nhận khi đăng nhập)
        - **Body**:
            ```json
                [{ 
                    "Email": "Thienthn@apzon.com",                          Email đăng nhập
                    "FullName":"Trần Thiên",                                Họ tên người dùng
                    "EmailValidate": "Thienthn@apzon.com",                  Email validate
                    "Phone": "0574654766",                                  Số điện thoại
                    "Address": "222 Trần Phú, P5, Q5"                         Địa chỉ
                }]
            ```    
        - **ReponseMessage**:
            ```json
            {
                "Content": 
                {
                    "Email": "Thienthn@apzon.com",
                    "FullName": "Trần Thiên",
                    "EmailValidate": "Thienthn@apzon.com",
                    "Phone": "0574654766",
                    "Address": "222 Trần Phú, P5, Q5"
                },
                "Message": "Updated Successfully.",
                "MessageCode": 200
            }
            ```
    # 07 API Update Password user       
    - **Path**: `/User/UpdatePassword`</br>
        Operation:
        - **Method**: `POST`
        - Note: Trả về thông báo.
        - **Authentications require**: `Bearer Token`
        - **Header**: Token: TokenId (Token nhận khi đăng nhập)
        - **Body**:
            ```json
                [{
                    "Email":"Thienthn@apzon.com",
                    "OldPassword":"1234567apzon",
                    "NewPassword":"55667799"
                }]
            ```    
        - **ReponseMessage**:
            ```json
            {
                "Content": null,
                "Message": "Updated Successfully.",
                "MessageCode": 200
            }
            ```
    # 08 API Request Reset Password       
    - **Path**: `/User/RequestResetPassword`</br>
        Operation:
        - **Method**: `POST`
        - Note: Trả về tokenid và thời gian hết hạn.
        - **Authentications require** : No
        - **Body**:
            ```json
            [{
                "Email":"Access6667@apzon.com"
            }]
            ```    
        - **ReponseMessage**:
            ```json
            {
                "Content": {
                    "TokenId": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6WyJBY2Nlc3M2NjY3QGFwem9uLmNvbSIsIjVjYTUyMjM2LWZhMjgtNGMwNC1hZWY3LTE3MDEyOGMxOTFhYiIsIjIwMjAxMDAyIl0sIm5iZiI6MTYwMTYwNjI2MywiZXhwIjoxNjAxNjA2OTIzLCJpYXQiOjE2MDE2MDYyNjN9.O8rGe2Dfs5lRcmFoXnFiYJGv2PQP-InvyKbou4fSHbQ",
                    "ExpiredTime": "2020-10-02T09:48:43.6363858+07:00"
                },
                "Message": "Request successfully.",
                "MessageCode": 200
            }
            ```
    # 09 API Reset Password       
    - **Path**: `/User/ResetPassword`</br>
        Operation:
        - **Method**: `POST`
        - Note: Trả về thông tin mật khẩu mới.
        - **Authentications require**: Null
        - **Body**:
            ```json
            [{
                "Token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6WyJBY2Nlc3M2NjY3QGFwem9uLmNvbSIsIjVjYTUyMjM2LWZhMjgtNGMwNC1hZWY3LTE3MDEyOGMxOTFhYiIsIjIwMjAxMDAyIl0sIm5iZiI6MTYwMTYwNjI2MywiZXhwIjoxNjAxNjA2OTIzLCJpYXQiOjE2MDE2MDYyNjN9.O8rGe2Dfs5lRcmFoXnFiYJGv2PQP-InvyKbou4fSHbQ"
            }]
            ```    
        - **ReponseMessage**:
            ```json
            {
                "Content": 
                {
                    "NewPassword": "e({7bP}{(j8*dSBU"
                },
                "Message": "Successful.",
                "MessageCode": 200
            }
            ```
    # Token Validate
    - **Path**: `/Token/Validate`</br>
    Operation: 
        - **Method**: `POST`
        - Note: Trả về thông tin token.
        - **Authentications require**: Null
        - **Body**:
            ```json
            { 
                "TokenID":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6WyJUaGFuaG52IiwiYjJkNGU3MzktZGUwZi00OTJkLTkxNzgtZTIxMmU5ZDFiN2YxIiwiMjAyMDEwMTUiXSwibmJmIjoxNjAwMTUzNjMxLCJleHAiOjE2MDI3NDU2MzAsImlhdCI6MTYwMDE1MzYzMX0.3L_DezY0ZYE_3b4XeEYBbsg6irYVLcGBU7HEHK8xqY8",
            }
            ```    
        - **ReponseMessage**:
            ```json
            {
                "Content":
                {
                    "Token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6WyJUaGFuaG52IiwiYjJkNGU3MzktZGUwZi00OTJkLTkxNzgtZTIxMmU5ZDFiN2YxIiwiMjAyMDEwMTUiXSwibmJmIjoxNjAwMTUzNjMxLCJleHAiOjE2MDI3NDU2MzAsImlhdCI6MTYwMDE1MzYzMX0.3L_DezY0ZYE_3b4XeEYBbsg6irYVLcGBU7HEHK8xqY8",
                    "UserName": "Thanhnv",
                    "ExpiredTime": "2020-10-15T14:07:10.143",
                    "Status":"R",
                    "Type":"N"
                },
                "Message": "Success",
                "MessageCode": 200
            }
            ```
    