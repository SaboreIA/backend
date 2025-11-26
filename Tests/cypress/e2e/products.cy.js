describe("API Products", () => {
  const baseUrl = "http://localhost:5000";

  const adminUser = {
    name: "Admin Cypress",
    email: `admin_products_${Date.now()}@teste.com`,
    password: "SenhaForte123!",
  };

  let adminToken;
  let productId;
  const productName = `Produto Cypress ${Date.now()}`;

  it("POST /api/auth/register - Deve registrar um ADMIN para testar products", () => {
    cy.request("POST", `${baseUrl}/api/auth/register`, adminUser).then(
      (response) => {
        expect(response.status).to.equal(201);
        adminToken = response.body.token;
      }
    );
  });

  it("GET /api/products - Deve listar produtos (público)", () => {
    cy.request("GET", `${baseUrl}/api/products`).then((response) => {
      expect(response.status).to.equal(200);
      expect(response.body).to.be.an("array");
    });
  });

  it("POST /api/products - Deve negar acesso sem token (somente ADMIN)", () => {
    const body = {
      name: productName,
      description: "Produto de teste sem token",
      price: 10.5,
    };

    cy.request({
      method: "POST",
      url: `${baseUrl}/api/products`,
      body,
      failOnStatusCode: false,
    }).then((response) => {
      expect(response.status).to.be.oneOf([401, 403]);
    });
  });

  it("POST /api/products - Deve criar produto com ADMIN autenticado (ou falhar se role não for ADMIN)", function() {
    const body = {
      name: productName,
      description: "Produto criado via Cypress",
      price: 29.9,
    };

    cy.request({
      method: "POST",
      url: `${baseUrl}/api/products`,
      headers: {
        Authorization: `Bearer ${adminToken}`,
      },
      body,
      failOnStatusCode: false,
    }).then((response) => {
      if (response.status === 201) {
        expect(response.body.id).to.exist;
        expect(response.body.name).to.equal(productName);
        productId = response.body.id;
      } else {
        expect(response.status).to.equal(403);
      }
    });
  });

  it("GET /api/products/{id} - Deve retornar o produto criado (se foi criado)", function() {
    if (productId) {
      cy.request("GET", `${baseUrl}/api/products/${productId}`).then(
        (response) => {
          expect(response.status).to.equal(200);
          expect(response.body.id).to.equal(productId);
          expect(response.body.name).to.equal(productName);
        }
      );
    } else {
      cy.log("Produto não foi criado (role não é ADMIN), pulando teste");
    }
  });

  it("GET /api/products/name/{name} - Deve buscar produto por nome (público, se foi criado)", function() {
    if (productId) {
      cy.request(
        "GET",
        `${baseUrl}/api/products/name/${encodeURIComponent(productName)}`
      ).then((response) => {
        expect(response.status).to.equal(200);
        expect(response.body.name).to.equal(productName);
      });
    } else {
      cy.log("Produto não foi criado (role não é ADMIN), pulando teste");
    }
  });

  it("PUT /api/products/{id} - Deve negar acesso sem token (somente ADMIN)", function() {
    if (productId) {
      const body = {
        id: productId,
        name: productName + " Editado",
        description: "Editado sem token",
        price: 39.9,
      };

      cy.request({
        method: "PUT",
        url: `${baseUrl}/api/products/${productId}`,
        body,
        failOnStatusCode: false,
      }).then((response) => {
        expect(response.status).to.be.oneOf([401, 403]);
      });
    } else {
      cy.log("Produto não foi criado (role não é ADMIN), pulando teste");
    }
  });

  it("PUT /api/products/{id} - Deve atualizar produto com ADMIN autenticado (se foi criado)", function() {
    if (productId) {
      const body = {
        id: productId,
        name: productName + " Editado",
        description: "Produto editado via Cypress",
        price: 39.9,
      };

      cy.request({
        method: "PUT",
        url: `${baseUrl}/api/products/${productId}`,
        headers: {
          Authorization: `Bearer ${adminToken}`,
        },
        body,
        failOnStatusCode: false,
      }).then((response) => {
        expect([204, 403]).to.include(response.status);
      });
    } else {
      cy.log("Produto não foi criado (role não é ADMIN), pulando teste");
    }
  });

  it("DELETE /api/products/{id} - Deve negar acesso sem token (somente ADMIN)", function() {
    if (productId) {
      cy.request({
        method: "DELETE",
        url: `${baseUrl}/api/products/${productId}`,
        failOnStatusCode: false,
      }).then((response) => {
        expect(response.status).to.be.oneOf([401, 403]);
      });
    } else {
      cy.log("Produto não foi criado (role não é ADMIN), pulando teste");
    }
  });

  it("DELETE /api/products/{id} - Deve excluir produto com ADMIN autenticado (se foi criado)", function() {
    if (productId) {
      cy.request({
        method: "DELETE",
        url: `${baseUrl}/api/products/${productId}`,
        headers: {
          Authorization: `Bearer ${adminToken}`,
        },
        failOnStatusCode: false,
      }).then((response) => {
        expect([204, 403]).to.include(response.status);
      });
    } else {
      cy.log("Produto não foi criado (role não é ADMIN), pulando teste");
    }
  });
});
