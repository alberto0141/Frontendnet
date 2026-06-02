# Pruebas de seguridad y funcionales – Frontend Frontendnet

**Proyecto:** Mercado Libre FEI — Frontend .NET 8 MVC  
**Fecha de elaboración:** 2026-06-02  
**Stack:** ASP.NET Core 8 MVC, Cookie Auth, JWT (almacenado en claim, no expuesto a JS)  
**Roles del sistema:** `Administrador`, `Usuario`

---

## Índice

1. [Validación de entrada (cliente)](#1-validación-de-entrada-cliente)
2. [Control de acceso y secciones privadas](#2-control-de-acceso-y-secciones-privadas)
3. [Protección CSRF](#3-protección-csrf)
4. [Flujos funcionales](#4-flujos-funcionales)
5. [Resumen de resultados](#5-resumen-de-resultados)

---

## Datos de prueba sugeridos

| Rol | Email | Contraseña |
|-----|-------|-----------|
| Administrador | `admin@fei.uv.mx` | La que tenga en el entorno |
| Usuario | `usuario@correo.com` | La que tenga en el entorno |

> Crea las cuentas antes de ejecutar las pruebas si no existen.

---

## 1. Validación de entrada (cliente)

> Estas pruebas verifican que el frontend rechaza datos inválidos **antes** de llegar al backend, mediante validación del modelo y atributos de DataAnnotations.

---

### 1.1 Registro con correo inválido

| Campo | Valor |
|-------|-------|
| **Objetivo** | El sistema rechaza un correo con formato incorrecto |
| **Ruta** | `/Auth/Registro` |
| **Datos de prueba** | Email: `no-es-un-correo`, Nombre: `Prueba Test`, Contraseña: `Test1234!`, Confirmar: `Test1234!` |
| **Resultado esperado** | El formulario muestra el mensaje de validación en el campo Email: *"El campo Correo electrónico no es un correo válido."* No se realiza ninguna petición al backend. |
| **Captura sugerida** | Formulario con campo Email resaltado en rojo mostrando el mensaje de error. |

**Pasos:**
1. Abre el navegador y navega a `/Auth/Registro`.
2. Ingresa `no-es-un-correo` en el campo **Correo electrónico**.
3. Ingresa `Prueba Test` en **Nombre**.
4. Ingresa `Test1234!` en **Contraseña** y **Confirmar contraseña**.
5. Haz clic en **Crear cuenta**.
6. Observa que el formulario **no se envía** y aparece el mensaje de error.

---

### 1.2 Registro con contraseña débil

| Campo | Valor |
|-------|-------|
| **Objetivo** | El sistema rechaza contraseñas que no cumplan complejidad mínima |
| **Ruta** | `/Auth/Registro` |
| **Datos de prueba** | Email: `prueba@correo.com`, Nombre: `Prueba Test`, Contraseña: `12345678`, Confirmar: `12345678` |
| **Resultado esperado** | El formulario muestra: *"La contraseña debe incluir mayúscula, minúscula, número y carácter especial, sin espacios."* |
| **Captura sugerida** | Campo Contraseña con mensaje de error visible. |

**Pasos:**
1. Navega a `/Auth/Registro`.
2. Rellena Email y Nombre con datos válidos.
3. Ingresa `12345678` en **Contraseña** (solo números, sin mayúscula, sin símbolo).
4. Repite `12345678` en **Confirmar contraseña**.
5. Haz clic en **Crear cuenta**.
6. Observa el mensaje de error sobre complejidad de la contraseña.

**Variantes adicionales para documentar:**

| Contraseña | Motivo del rechazo |
|------------|-------------------|
| `test` | Muy corta (< 8 chars) |
| `testtest` | Sin mayúscula, número ni símbolo |
| `Testtest1` | Sin carácter especial |
| `Test 123!` | Contiene espacio |

---

### 1.3 Categoría con nombre que contiene caracteres inválidos

| Campo | Valor |
|-------|-------|
| **Objetivo** | El sistema rechaza caracteres especiales no permitidos en nombre de categoría |
| **Ruta** | `/Categorias/Crear` (requiere rol Administrador) |
| **Datos de prueba** | Nombre: `Electrónica (nueva)` |
| **Resultado esperado** | Error: *"El nombre solo puede contener letras, números, espacios, punto, coma y guion. No use paréntesis ni caracteres especiales."* |
| **Captura sugerida** | Campo Nombre de categoría con mensaje de error visible. |

**Pasos:**
1. Inicia sesión como **Administrador**.
2. Navega a `/Categorias/Crear`.
3. Ingresa `Electrónica (nueva)` en el campo **Nombre** (incluye paréntesis).
4. Haz clic en **Guardar**.
5. Observa el mensaje de validación.

**Variante válida para contraste:** Ingresa `Electrónica-nueva` y verifica que sí se acepta.

---

### 1.4 Producto con título vacío

| Campo | Valor |
|-------|-------|
| **Objetivo** | El sistema rechaza un producto sin título |
| **Ruta** | `/Productos/Crear` (requiere rol Administrador) |
| **Datos de prueba** | Título: *(vacío)*, Descripción: `Descripción de prueba`, Precio: `199.99` |
| **Resultado esperado** | Error en campo Título: *"El campo Título es obligatorio."* |
| **Captura sugerida** | Formulario con campo Título resaltado y mensaje de requerido. |

**Pasos:**
1. Inicia sesión como **Administrador**.
2. Navega a `/Productos/Crear`.
3. Deja el campo **Título** vacío.
4. Rellena **Descripción** y **Precio** con valores válidos.
5. Haz clic en **Guardar**.
6. Observa el error de validación en Título.

---

### 1.5 Producto con precio negativo o cero

| Campo | Valor |
|-------|-------|
| **Objetivo** | El sistema rechaza precios fuera del rango válido (0.01 – 999,999.99) |
| **Ruta** | `/Productos/Crear` (requiere rol Administrador) |
| **Datos de prueba (caso 1)** | Título: `Laptop Test`, Descripción: `Desc`, Precio: `-10` |
| **Datos de prueba (caso 2)** | Mismo, Precio: `0` |
| **Resultado esperado** | Error en campo Precio: *"El campo Precio debe ser mayor a 0 y tener un valor válido."* |
| **Captura sugerida** | Campo Precio con mensaje de error y valor inválido visible. |

**Pasos:**
1. Inicia sesión como **Administrador**.
2. Navega a `/Productos/Crear`.
3. Rellena Título y Descripción con datos válidos.
4. Ingresa `-10` en el campo **Precio**.
5. Haz clic en **Guardar**.
6. Verifica el mensaje de error.
7. Repite con `0`.

---

### 1.6 Subir archivo de tipo incorrecto (PNG/WebP en lugar de JPG)

| Campo | Valor |
|-------|-------|
| **Objetivo** | El sistema rechaza formatos de imagen no permitidos |
| **Ruta** | `/Archivos/Crear` (requiere rol Administrador) |
| **Datos de prueba** | Archivo: `imagen.png` (o `.webp`) |
| **Resultado esperado** | Error: *"Solo se permiten imágenes JPG/JPEG."* El archivo no se sube. |
| **Captura sugerida** | Formulario de carga con error de tipo de archivo visible. |

**Pasos:**
1. Inicia sesión como **Administrador**.
2. Navega a `/Archivos/Crear`.
3. Selecciona un archivo `.png` o `.webp` en el campo de portada.
4. Rellena los demás campos requeridos.
5. Haz clic en **Guardar**.
6. Observa el mensaje de error sobre el tipo de archivo.

**Variante válida para contraste:** Selecciona un archivo `.jpg` válido (tamaño < 2 MB) y verifica que sí se acepta.

> **Nota de implementación:** La validación de extensión y Content-Type está en `Services/ArchivosClientService.cs`. Las extensiones permitidas son: `.jpg`, `.jpeg`. El tamaño máximo es **2 MB**.

---

## 2. Control de acceso y secciones privadas

> Estas pruebas verifican que las rutas están protegidas correctamente según el rol del usuario.

---

### 2.1 Usuario no autenticado intenta acceder al carrito

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar redirección al login para rutas protegidas |
| **Ruta atacada** | `/Carrito` |
| **Resultado esperado** | Redirección automática a `/Auth?returnUrl=%2FCarrito`. El carrito **no es accesible**. |
| **Captura sugerida** | Barra de dirección mostrando la URL de login con el parámetro `returnUrl`. |

**Pasos:**
1. Abre una ventana de incógnito (o cierra sesión).
2. Navega directamente a `/Carrito`.
3. Observa la redirección automática a la página de login.
4. Verifica que en la URL aparece `?returnUrl=%2FCarrito`.

---

### 2.2 Usuario con rol `Usuario` intenta acceder a gestión de usuarios

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que el rol `Usuario` no puede acceder a rutas de administración |
| **Ruta atacada** | `/Usuarios` |
| **Resultado esperado** | Redirección a la página de acceso denegado (`/Home/AccessDenied`). **No se muestra ningún dato**. |
| **Captura sugerida** | Página de acceso denegado visible después del intento. |

**Pasos:**
1. Inicia sesión con una cuenta de rol **Usuario**.
2. Navega manualmente a `/Usuarios`.
3. Observa la redirección a la página de acceso denegado.

**Rutas adicionales a intentar con rol Usuario:**

| Ruta | Resultado esperado |
|------|--------------------|
| `/Usuarios` | Acceso denegado |
| `/Categorias` | Acceso denegado |
| `/Archivos` | Acceso denegado |
| `/Bitacora` | Acceso denegado |
| `/Pedidos/AdminIndex` | Acceso denegado |
| `/Productos/Crear` | Acceso denegado |
| `/Productos/Editar/1` | Acceso denegado |
| `/Productos/Eliminar/1` | Acceso denegado |

---

### 2.3 Administrador intenta acceder al carrito de compras

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que el rol `Administrador` no puede usar el carrito (solo `Usuario`) |
| **Ruta atacada** | `/Carrito` |
| **Resultado esperado** | Redirección a acceso denegado. El carrito es exclusivo del rol `Usuario`. |
| **Captura sugerida** | Página de acceso denegado visible. |

**Pasos:**
1. Inicia sesión como **Administrador**.
2. Navega manualmente a `/Carrito`.
3. Observa la redirección a acceso denegado.

> **Fundamento:** `CarritoController` tiene `[Authorize(Roles = AppRoles.User)]` a nivel de clase (solo `"Usuario"`).

---

### 2.4 El menú muestra opciones según el rol

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que el menú de navegación respeta el rol del usuario autenticado |
| **Resultado esperado (Usuario)** | Menú muestra: Comprar, Carrito, Mis pedidos. **No** muestra: Productos, Categorías, Archivos, Usuarios, Bitácora. |
| **Resultado esperado (Administrador)** | Menú muestra: Productos, Categorías, Archivos, Usuarios, Bitácora, Pedidos. **No** muestra: Comprar, Carrito, Mis pedidos. |
| **Captura sugerida** | Screenshot del menú de navegación de cada rol. |

**Pasos – rol Usuario:**
1. Inicia sesión con rol **Usuario**.
2. Toma captura de pantalla del menú superior.
3. Verifica que **solo** aparecen Comprar, Carrito y Mis pedidos.

**Pasos – rol Administrador:**
1. Inicia sesión con rol **Administrador**.
2. Toma captura del menú superior.
3. Verifica que **solo** aparecen Productos, Categorías, Archivos, Usuarios, Bitácora y Pedidos.

---

### 2.5 Sin sesión no hay menú de navegación privado

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que un visitante sin sesión no ve opciones privadas |
| **Resultado esperado** | El menú muestra únicamente el enlace **Inicio**. No hay Comprar, Carrito, Usuarios, etc. |
| **Captura sugerida** | Menú superior mostrando solo "Inicio". |

**Pasos:**
1. Cierra sesión o abre una ventana de incógnito.
2. Navega a la página principal.
3. Verifica el menú de navegación.

---

## 3. Protección CSRF

> Estas pruebas verifican que los formularios POST están protegidos con tokens antifalsificación.

---

### 3.1 Presencia del token antiforgery en formularios

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que todos los formularios POST incluyen el campo `__RequestVerificationToken` |
| **Resultado esperado** | Cada formulario POST contiene un `<input type="hidden" name="__RequestVerificationToken" value="...">` |
| **Captura sugerida** | Panel de DevTools (Elements) mostrando el campo oculto en el formulario. |

**Formularios a verificar:**

| Vista | Acción | Token presente |
|-------|--------|:--------------:|
| `/Auth` | Login | ✓ |
| `/Auth/Registro` | Crear cuenta | ✓ |
| `/Productos/Crear` | Crear producto | ✓ |
| `/Productos/Editar/{id}` | Editar producto | ✓ |
| `/Productos/Eliminar/{id}` | Eliminar producto | ✓ |
| `/Categorias/Crear` | Crear categoría | ✓ |
| `/Categorias/Editar/{id}` | Editar categoría | ✓ |
| `/Categorias/Eliminar/{id}` | Eliminar categoría | ✓ |
| `/Usuarios/Crear` | Crear usuario | ✓ |
| `/Usuarios/Editar/{id}` | Editar usuario | ✓ |
| `/Usuarios/Eliminar/{id}` | Eliminar usuario | ✓ |
| `/Archivos/Crear` | Subir archivo | ✓ |
| `/Archivos/Editar/{id}` | Editar archivo | ✓ |
| `/Archivos/Eliminar/{id}` | Eliminar archivo | ✓ |
| `/Comprar` | Agregar al carrito | ✓ |
| `/Carrito` | Incrementar, Decrementar, Eliminar, Vaciar, Confirmar compra | ✓ |
| `/Pedidos/Detalle/{id}` | Cambiar estado (admin) | ✓ |

**Pasos para verificar un formulario específico:**
1. Inicia sesión con el rol correspondiente.
2. Navega al formulario objetivo.
3. Abre las herramientas de desarrollo del navegador (`F12`).
4. Ve a la pestaña **Elements** (o **Inspector**).
5. Busca dentro del `<form>` el campo:
   ```html
   <input name="__RequestVerificationToken" type="hidden" value="CfD..." />
   ```
6. Confirma que el campo está presente.

---

### 3.2 POST sin token antiforgery es rechazado

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que el servidor rechaza peticiones POST sin token CSRF |
| **Resultado esperado** | Respuesta HTTP **400 Bad Request**. La operación no se realiza. |
| **Captura sugerida** | Pestaña Network de DevTools mostrando la respuesta 400. |

**Pasos (usando la consola del navegador):**
1. Inicia sesión con cualquier rol.
2. Navega al carrito (`/Carrito`).
3. Abre la consola del navegador (`F12` → Console).
4. Ejecuta la siguiente petición sin token:
   ```javascript
   fetch('/Carrito/Vaciar', {
     method: 'POST',
     headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
     body: ''
   }).then(r => console.log('Status:', r.status));
   ```
5. Observa en la consola y en la pestaña **Network** que la respuesta es **400**.

> **Fundamento:** La aplicación registra `AutoValidateAntiforgeryTokenAttribute` globalmente en `Program.cs` (línea 84), y además cada action POST tiene `[ValidateAntiForgeryToken]` individualmente. Ambas capas rechazan peticiones sin token válido.

---

### 3.3 El token antiforgery usa cookie HttpOnly + SameSite=Strict

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar configuración segura de la cookie antiforgery |
| **Resultado esperado** | Cookie `frontendnet.antiforgery` con flags `HttpOnly`, `SameSite=Strict` |
| **Captura sugerida** | Panel de Cookies en DevTools mostrando los atributos de la cookie. |

**Pasos:**
1. Navega a cualquier página de la aplicación.
2. Abre DevTools → pestaña **Application** → **Cookies**.
3. Selecciona el dominio `localhost`.
4. Busca la cookie `frontendnet.antiforgery`.
5. Verifica los atributos: `HttpOnly: true`, `SameSite: Strict`.

---

## 4. Flujos funcionales

> Estas pruebas verifican que los flujos de usuario principales funcionan correctamente de extremo a extremo.

---

### 4.1 Login de usuario

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que el login funciona y redirige según el rol |
| **Datos de prueba** | Email y contraseña de una cuenta válida con rol `Usuario` |
| **Resultado esperado** | Login exitoso → redirección a `/Home`. Menú muestra Comprar, Carrito, Mis pedidos. |
| **Captura sugerida** | Pantalla de Home después del login con el nombre del usuario visible y menú correcto. |

**Pasos:**
1. Navega a `/Auth`.
2. Ingresa email y contraseña de un **Usuario** válido.
3. Haz clic en **Iniciar sesión**.
4. Verifica redirección a `/Home`.
5. Verifica que el nombre del usuario aparece en la barra de navegación.

---

### 4.2 Login de administrador

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que el admin es redirigido a la lista de productos |
| **Datos de prueba** | Email y contraseña de una cuenta con rol `Administrador` |
| **Resultado esperado** | Login exitoso → redirección a `/Productos`. Menú muestra opciones de administración. |
| **Captura sugerida** | Lista de productos visible después del login admin. |

**Pasos:**
1. Navega a `/Auth`.
2. Ingresa credenciales del **Administrador**.
3. Haz clic en **Iniciar sesión**.
4. Verifica redirección a `/Productos`.
5. Verifica que el menú muestra las opciones de administración.

---

### 4.3 Agregar producto al carrito

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que un usuario puede agregar un producto desde el catálogo |
| **Requisito previo** | Al menos un producto disponible en el sistema |
| **Resultado esperado** | El producto aparece en el carrito. Mensaje de éxito: *"'[nombre]' agregado al carrito."* |
| **Captura sugerida** | Carrito con el producto recién agregado y el mensaje de éxito visible. |

**Pasos:**
1. Inicia sesión como **Usuario**.
2. Navega a `/Comprar`.
3. Localiza cualquier producto en la lista.
4. Haz clic en **Agregar al carrito**.
5. Observa el mensaje de confirmación.
6. Navega a `/Carrito` y verifica que el producto aparece.

---

### 4.4 Cambiar cantidad en el carrito (incrementar y decrementar)

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que los botones +/- modifican correctamente la cantidad |
| **Requisito previo** | Al menos un producto en el carrito |
| **Resultado esperado** | La cantidad aumenta o disminuye correctamente. El botón `-` se deshabilita cuando la cantidad es 1. |
| **Captura sugerida** | Carrito con cantidad modificada y subtotal actualizado. |

**Pasos:**
1. Agrega un producto al carrito (ver prueba 4.3).
2. En `/Carrito`, haz clic en el botón **+** del producto.
3. Verifica que la cantidad cambia de 1 a 2 y el subtotal se actualiza.
4. Haz clic en el botón **-**.
5. Verifica que vuelve a 1.
6. Verifica que el botón **-** está deshabilitado con cantidad 1.

---

### 4.5 Confirmar pedido (compra completa)

| Campo | Valor |
|-------|-------|
| **Objetivo** | Verificar el flujo completo de compra desde carrito hasta pedido |
| **Requisito previo** | Al menos un producto en el carrito |
| **Resultado esperado** | Pedido creado correctamente → redirección al detalle del pedido con mensaje *"¡Pedido creado correctamente!"* El carrito queda vacío. |
| **Captura sugerida** | Pantalla de detalle del pedido con ID, estado PENDIENTE, total y productos. |

**Pasos:**
1. Inicia sesión como **Usuario**.
2. Agrega uno o más productos al carrito.
3. En `/Carrito`, haz clic en **Confirmar compra**.
4. Observa la redirección al detalle del pedido.
5. Verifica: estado `PENDIENTE`, total correcto, productos listados.
6. Regresa al carrito y verifica que está vacío.

---

### 4.6 Ver mis pedidos

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que el usuario puede consultar su historial de pedidos |
| **Requisito previo** | Al menos un pedido creado |
| **Resultado esperado** | Lista de pedidos propios con fecha, estado y total. Enlace al detalle de cada pedido. |
| **Captura sugerida** | Tabla de pedidos con al menos un registro. |

**Pasos:**
1. Inicia sesión como **Usuario** (que haya hecho pedidos).
2. Navega a `/Pedidos`.
3. Verifica que la lista muestra los pedidos del usuario.
4. Haz clic en **Ver detalle** de cualquier pedido.
5. Verifica que se muestra correctamente el detalle.

---

### 4.7 Ver todos los pedidos (Admin)

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que el admin puede ver pedidos de todos los usuarios |
| **Resultado esperado** | Lista de pedidos de todos los usuarios con email del comprador visible. |
| **Captura sugerida** | Tabla de pedidos con columna de usuario/email. |

**Pasos:**
1. Inicia sesión como **Administrador**.
2. En el menú, haz clic en **Pedidos** (que apunta a `/Pedidos/AdminIndex`).
3. Verifica que la lista muestra pedidos con el email del comprador.

---

### 4.8 Cambiar estado de un pedido (Admin)

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que el admin puede cambiar el estado de un pedido |
| **Estados disponibles** | `PENDIENTE`, `EN_PROCESO`, `ENVIADO`, `ENTREGADO`, `CANCELADO` |
| **Resultado esperado** | Estado del pedido actualizado correctamente. Mensaje de éxito visible. Badge del estado cambia de color. |
| **Captura sugerida** | Detalle del pedido con el nuevo estado y badge de color correspondiente. |

**Pasos:**
1. Inicia sesión como **Administrador**.
2. Navega al detalle de cualquier pedido.
3. En la sección **Cambiar estado**, selecciona `EN_PROCESO`.
4. Haz clic en **Actualizar estado**.
5. Verifica el mensaje de éxito y el nuevo estado del pedido.
6. Repite con `ENVIADO`, `ENTREGADO`.

---

### 4.9 Crear producto

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que un admin puede crear un producto nuevo |
| **Datos de prueba** | Título: `Laptop Gaming Test`, Descripción: `Descripción de prueba para evidencia`, Precio: `15999.00` |
| **Resultado esperado** | Producto creado y visible en la lista de productos. |
| **Captura sugerida** | Formulario de creación completado y lista de productos con el nuevo producto. |

**Pasos:**
1. Inicia sesión como **Administrador**.
2. Navega a `/Productos/Crear`.
3. Rellena los campos con los datos de prueba.
4. Haz clic en **Guardar**.
5. Verifica que el producto aparece en la lista `/Productos`.

---

### 4.10 Editar producto

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que un admin puede modificar un producto existente |
| **Requisito previo** | Producto creado en prueba 4.9 |
| **Datos de prueba** | Cambiar Precio a `17999.00`, descripción a `Descripción actualizada` |
| **Resultado esperado** | Cambios guardados correctamente. El detalle muestra los datos actualizados. |
| **Captura sugerida** | Detalle del producto con los datos actualizados. |

**Pasos:**
1. Inicia sesión como **Administrador**.
2. En la lista de productos, haz clic en **Editar** del producto creado.
3. Modifica el precio y la descripción.
4. Haz clic en **Guardar**.
5. Verifica los cambios en el detalle del producto.

---

### 4.11 Eliminar producto

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que un admin puede eliminar un producto |
| **Requisito previo** | Producto creado en prueba 4.9 |
| **Resultado esperado** | Producto eliminado y ya no aparece en la lista. |
| **Captura sugerida** | Lista de productos sin el producto eliminado. |

**Pasos:**
1. Inicia sesión como **Administrador**.
2. En la lista de productos, haz clic en **Eliminar** del producto de prueba.
3. En la pantalla de confirmación, confirma la eliminación.
4. Verifica que el producto ya no aparece en la lista.

---

### 4.12 Crear categoría

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que un admin puede crear una nueva categoría |
| **Datos de prueba** | Nombre: `Tecnologia-Test` |
| **Resultado esperado** | Categoría creada y visible en la lista. |
| **Captura sugerida** | Lista de categorías con la nueva categoría. |

**Pasos:**
1. Inicia sesión como **Administrador**.
2. Navega a `/Categorias/Crear`.
3. Ingresa `Tecnologia-Test` en el campo **Nombre**.
4. Haz clic en **Guardar**.
5. Verifica que aparece en la lista.

---

### 4.13 Editar y eliminar categoría

**Editar:**
1. En la lista de categorías, haz clic en **Editar** de `Tecnologia-Test`.
2. Cambia el nombre a `Tecnologia-v2`.
3. Guarda y verifica el cambio.

**Eliminar:**
1. Haz clic en **Eliminar** de la categoría de prueba.
2. Confirma la eliminación.
3. Verifica que no aparece en la lista.

| **Captura sugerida** | Lista de categorías después de la eliminación. |

---

### 4.14 Crear y editar usuario (Admin)

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que el admin puede gestionar usuarios |
| **Datos de prueba** | Email: `test.usuario@fei.uv.mx`, Nombre: `Test Usuario`, Rol: `Usuario`, Contraseña: `Test1234!` |
| **Resultado esperado** | Usuario creado y visible en la lista. Edición guarda los cambios correctamente. |
| **Captura sugerida** | Lista de usuarios con el nuevo usuario; formulario de edición. |

**Pasos – Crear:**
1. Inicia sesión como **Administrador**.
2. Navega a `/Usuarios/Crear`.
3. Rellena los campos con los datos de prueba.
4. Haz clic en **Guardar**.
5. Verifica que el usuario aparece en la lista.

**Pasos – Editar:**
1. Haz clic en **Editar** del usuario recién creado.
2. Cambia el nombre a `Test Usuario Editado`.
3. Guarda y verifica el cambio.

---

### 4.15 Subir archivo JPG válido

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que la carga de imágenes JPG funciona correctamente |
| **Datos de prueba** | Archivo: `imagen-prueba.jpg` (< 2 MB) |
| **Resultado esperado** | Archivo subido correctamente y visible en la lista de archivos con previsualización. |
| **Captura sugerida** | Lista de archivos con la imagen subida; vista de detalle con previsualización. |

**Pasos:**
1. Inicia sesión como **Administrador**.
2. Navega a `/Archivos/Crear`.
3. Selecciona un archivo `.jpg` válido (< 2 MB).
4. Haz clic en **Guardar**.
5. Verifica que el archivo aparece en la lista con su previsualización.

---

### 4.16 Ver bitácora (Admin)

| Campo | Valor |
|-------|-------|
| **Objetivo** | Confirmar que el admin puede consultar el registro de auditoría |
| **Resultado esperado** | Lista de eventos del sistema con fecha, acción y usuario responsable. |
| **Captura sugerida** | Tabla de bitácora con registros visibles. |

**Pasos:**
1. Inicia sesión como **Administrador**.
2. Navega a `/Bitacora`.
3. Verifica que se muestra la lista de registros del sistema.

---

## 5. Resumen de resultados

Llena esta tabla durante la ejecución de las pruebas.

| # | Prueba | Estado | Observaciones |
|---|--------|:------:|---------------|
| 1.1 | Registro con correo inválido | ⬜ | |
| 1.2 | Registro con contraseña débil | ⬜ | |
| 1.3 | Categoría con caracteres inválidos | ⬜ | |
| 1.4 | Producto con título vacío | ⬜ | |
| 1.5 | Producto con precio negativo | ⬜ | |
| 1.6 | Archivo PNG rechazado | ⬜ | |
| 2.1 | Sin login → carrito redirige a login | ⬜ | |
| 2.2 | Usuario normal no accede a /Usuarios | ⬜ | |
| 2.3 | Admin no accede al carrito | ⬜ | |
| 2.4 | Menú muestra opciones según rol | ⬜ | |
| 2.5 | Sin sesión menú solo muestra Inicio | ⬜ | |
| 3.1 | Token antiforgery presente en formularios | ⬜ | |
| 3.2 | POST sin token devuelve 400 | ⬜ | |
| 3.3 | Cookie antiforgery con HttpOnly + SameSite=Strict | ⬜ | |
| 4.1 | Login de usuario | ⬜ | |
| 4.2 | Login de administrador | ⬜ | |
| 4.3 | Agregar producto al carrito | ⬜ | |
| 4.4 | Cambiar cantidad en carrito | ⬜ | |
| 4.5 | Confirmar pedido | ⬜ | |
| 4.6 | Ver mis pedidos | ⬜ | |
| 4.7 | Ver todos los pedidos (admin) | ⬜ | |
| 4.8 | Cambiar estado de pedido | ⬜ | |
| 4.9 | Crear producto | ⬜ | |
| 4.10 | Editar producto | ⬜ | |
| 4.11 | Eliminar producto | ⬜ | |
| 4.12 | Crear categoría | ⬜ | |
| 4.13 | Editar y eliminar categoría | ⬜ | |
| 4.14 | Crear y editar usuario | ⬜ | |
| 4.15 | Subir archivo JPG válido | ⬜ | |
| 4.16 | Ver bitácora | ⬜ | |

**Leyenda:** ✅ Aprobado · ❌ Fallido · ⚠️ Parcial · ⬜ Pendiente

---

## Hallazgos de seguridad

Durante el análisis del código se verificaron los siguientes controles de seguridad:

| Control | Estado | Detalle |
|---------|:------:|---------|
| Protección CSRF global | ✅ | `AutoValidateAntiforgeryTokenAttribute` en `Program.cs` |
| Validación CSRF por action | ✅ | `[ValidateAntiForgeryToken]` en todos los POST |
| Cookie auth: HttpOnly | ✅ | `options.Cookie.HttpOnly = true` |
| Cookie auth: SameSite=Strict | ✅ | `options.Cookie.SameSite = SameSiteMode.Strict` |
| Cookie auth: Secure en prod | ✅ | `CookieSecurePolicy.Always` en producción |
| Headers de seguridad: CSP | ✅ | Configurado en middleware |
| Headers: X-Frame-Options DENY | ✅ | Configurado en middleware |
| Headers: X-Content-Type-Options | ✅ | `nosniff` configurado |
| Mensajes técnicos expuestos | ✅ | Sin exposición — todos los mensajes al usuario son genéricos |
| DeveloperExceptionPage | ✅ | Solo activo en entorno `Development` |
| Rutas de admin protegidas | ✅ | `[Authorize(Roles = AppRoles.Administrator)]` en todos los controllers admin |
| JWT no expuesto a JavaScript | ✅ | Almacenado en claim de cookie HttpOnly, no accesible desde JS |
| Validación de tipo de archivo | ✅ | Extensión + Content-Type verificados (solo JPG/JPEG) |
| Tamaño máximo de archivo | ✅ | 2 MB configurado en `FormOptions` y validado en servicio |
| Admin no puede eliminarse | ✅ | Validación en `UsuariosController` |
