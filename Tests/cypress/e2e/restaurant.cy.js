describe("API Restaurants", () => {
  const baseUrl = "http://localhost:5000";

  const user = {
    name: "Dono Restaurante Cypress",
    email: `owner_rest_${Date.now()}@teste.com`,
    password: "SenhaForte123!",
  };

  let token;
  let restaurantId;

  it("GET /api/restaurants - Deve listar restaurantes (público, paginado)", () => {
    cy.request("GET", `${baseUrl}/api/restaurants`).then((response) => {
      expect(response.status).to.equal(200);
      expect(response.body.items || response.body.data || response.body).to.exist;
    });
  });

  it("GET /api/restaurants/{id} - Deve tentar buscar restaurante inexistente e poder retornar 404", () => {
    cy.request({
      method: "GET",
      url: `${baseUrl}/api/restaurants/999999`,
      failOnStatusCode: false,
    }).then((response) => {
      expect([200, 404]).to.include(response.status);
    });
  });

  it("GET /api/restaurants/owner/{ownerId} - Deve ser pública", () => {
    cy.request("GET", `${baseUrl}/api/restaurants/owner/1`).then((response) => {
      expect([200, 404]).to.include(response.status);
    });
  });

  it("POST /api/restaurants/search - Deve permitir busca por IA (público)", () => {
    const body = {
      userInput: "pizza",
      city: "Marilia",
    };

    cy.request("POST", `${baseUrl}/api/restaurants/search`, body).then(
      (response) => {
        expect([200, 404]).to.include(response.status);
      }
    );
  });

  it("POST /api/auth/register - Deve registrar usuário para criar restaurante", () => {
    cy.request("POST", `${baseUrl}/api/auth/register`, user).then((response) => {
      expect(response.status).to.equal(201);
      token = response.body.token;
    });
  });

  it("POST /api/restaurants - Deve negar criação sem token (rota protegida)", () => {
    const body = {
      name: "Restaurante Sem Token",
      description: "Não deve ser criado",
      openTime: "09:00",
      closeTime: "22:00",
      address: {
        zipCode: "17500-000",
        street: "Rua Teste",
        number: "123",
        city: "Marilia",
        state: "SP",
        country: "Brasil"
      }
    };

    cy.request({
      method: "POST",
      url: `${baseUrl}/api/restaurants`,
      body,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("POST /api/restaurants - Deve criar restaurante com usuário autenticado", function() {
    const body = {
      name: `Restaurante Cypress ${Date.now()}`,
      description: "Criado via teste e2e",
      openTime: "09:00",
      closeTime: "22:00",
      address: {
        zipCode: "17500-000",
        street: "Rua Cypress",
        number: "100",
        city: "Marilia",
        state: "SP",
        country: "Brasil"
      }
    };

    cy.request({
      method: "POST",
      url: `${baseUrl}/api/restaurants`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body,
    }).then((response) => {
      expect(response.status).to.equal(201);
      expect(response.body.id).to.exist;
      restaurantId = response.body.id;
    });
  });

  it("GET /api/restaurants/{id} - Deve retornar restaurante recém-criado (público)", function() {
    cy.request("GET", `${baseUrl}/api/restaurants/${restaurantId}`).then(
      (response) => {
        expect(response.status).to.equal(200);
        expect(response.body.id).to.equal(restaurantId);
      }
    );
  });

  it("PUT /api/restaurants/{id} - Deve negar atualização sem token", function() {
    const body = {
      name: "Nome editado sem token",
      address: {
        zipCode: "17500-001",
        street: "Rua Teste",
        number: "456",
        city: "Marilia",
        state: "SP",
        country: "Brasil"
      }
    };

    cy.request({
      method: "PUT",
      url: `${baseUrl}/api/restaurants/${restaurantId}`,
      body,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("PUT /api/restaurants/{id} - Deve atualizar restaurante com proprietário autenticado", function() {
    const body = {
      name: "Restaurante Cypress Editado",
      description: "Editado via Cypress",
      openTime: "10:00",
      closeTime: "23:00",
      address: {
        zipCode: "17500-001",
        street: "Rua Cypress Editada",
        number: "200",
        city: "Marilia",
        state: "SP",
        country: "Brasil"
      }
    };

    cy.request({
      method: "PUT",
      url: `${baseUrl}/api/restaurants/${restaurantId}`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body,
    }).then((response) => {
      expect(response.status).to.equal(200);
    });
  });

  it("POST /api/restaurants/{id}/upload-all-images - Deve negar sem token", function() {
    cy.request({
      method: "POST",
      url: `${baseUrl}/api/restaurants/${restaurantId}/upload-all-images`,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("POST /api/restaurants/{id}/upload-all-images - Deve aceitar com proprietário autenticado (se multipart estiver configurado)", function() {
    cy.request({
      method: "POST",
      url: `${baseUrl}/api/restaurants/${restaurantId}/upload-all-images`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
      failOnStatusCode: false,
    }).then((response) => {
      expect([200, 400, 415]).to.include(response.status);
    });
  });

  it("DELETE /api/restaurants/{id} - Deve negar exclusão sem token", function() {
    cy.request({
      method: "DELETE",
      url: `${baseUrl}/api/restaurants/${restaurantId}`,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("DELETE /api/restaurants/{id} - Deve excluir restaurante com proprietário autenticado", function() {
    cy.request({
      method: "DELETE",
      url: `${baseUrl}/api/restaurants/${restaurantId}`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }).then((response) => {
      expect(response.status).to.equal(204);
    });
  });
});
