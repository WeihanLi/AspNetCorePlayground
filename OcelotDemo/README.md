## Ocelot Demo Project

## Intro

这是一个 ocelot 的示例项目，做了一些精简和使用了自定义的一个认证授权

## Play

1. 启动项目
2. 访问 `/api/test/values`，应该可以直接拿到一个正常的 response，response status code 是 200
3. 访问 `/api/test/user` ，这个路由配置了需要用户登录才能访问，直接访问这个路径会 401
4. 访问 `/api/test/user?userId=1&userName=test&userRoles=User` ，应该会返回200
5. 访问 `/api/test/admin?userId=1&userName=test&userRoles=User` ，应该会返回403，因为这个路径需要 `Admin` 角色
6. 访问 `/api/test/admin?userId=1&userName=test&userRoles=Admin`，正常返回 ，应该会返回200

