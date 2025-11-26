describe("API Review", () => {
  const baseUrl = "http://localhost:5000";

  const user = {
    name: "Reviewer Cypress",
    email: `reviewer_${Date.now()}@teste.com`,
    password: "SenhaForte123!",
  };

  let token;
  let reviewId;
  const restaurantId = 1;

  it("GET /api/review - Deve listar avaliações (público, paginado)", () => {
    cy.request("GET", `${baseUrl}/api/review`).then((response) => {
      expect(response.status).to.equal(200);
    });
  });

  it("GET /api/review/{id} - Deve tentar buscar review inexistente e poder retornar 404", () => {
    cy.request({
      method: "GET",
      url: `${baseUrl}/api/review/999999`,
      failOnStatusCode: false,
    }).then((response) => {
      expect([200, 404]).to.include(response.status);
    });
  });

  it("GET /api/review/restaurant/{restaurantId} - Deve ser pública (paginada)", () => {
    cy.request(
      "GET",
      `${baseUrl}/api/review/restaurant/${restaurantId}?pageNumber=1&pageSize=10`
    ).then((response) => {
      expect([200, 404]).to.include(response.status);
    });
  });

  it("GET /api/review/user/{userId} - Deve ser pública (paginada)", () => {
    cy.request(
      "GET",
      `${baseUrl}/api/review/user/1?pageNumber=1&pageSize=10`
    ).then((response) => {
      expect([200, 404]).to.include(response.status);
    });
  });

  it("GET /api/review/restaurant/{restaurantId}/average - Deve ser pública", () => {
    cy.request(
      "GET",
      `${baseUrl}/api/review/restaurant/${restaurantId}/average`
    ).then((response) => {
      expect([200, 404]).to.include(response.status);
    });
  });

  it("POST /api/auth/register - Deve registrar usuário para criar review", () => {
    cy.request("POST", `${baseUrl}/api/auth/register`, user).then((response) => {
      expect(response.status).to.equal(201);
      token = response.body.token;
    });
  });

  it("POST /api/review - Deve negar criação sem token (rota protegida)", () => {
    const body = {
      restaurantId,
      title: "Review sem token",
      comment: "Comentário",
      rating1: 5,
      rating2: 5,
      rating3: 5,
      rating4: 5,
    };

    cy.request({
      method: "POST",
      url: `${baseUrl}/api/review`,
      body,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("POST /api/review - Deve criar review com usuário autenticado", function () {
    const body = {
      restaurantId,
      title: "Excelente Restaurante",
      comment: "Review criada via Cypress",
      rating1: 5,
      rating2: 4,
      rating3: 5,
      rating4: 4,
    };

    cy.request({
      method: "POST",
      url: `${baseUrl}/api/review`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body,
    }).then((response) => {
      expect(response.status).to.equal(201);
      expect(response.body.id).to.exist;
      expect(response.body.restaurantId).to.equal(restaurantId);
      reviewId = response.body.id;
    });
  });

  it("GET /api/review/{id} - Deve retornar review recém-criada (público)", function () {
    cy.request("GET", `${baseUrl}/api/review/${reviewId}`).then((response) => {
      expect(response.status).to.equal(200);
      expect(response.body.id).to.equal(reviewId);
      expect(response.body.restaurantId).to.equal(restaurantId);
    });
  });

  it("PUT /api/review/{id} - Deve negar atualização sem token", function () {
    const body = {
      title: "Título editado",
      comment: "Editado sem token",
      rating1: 4,
      rating2: 4,
      rating3: 4,
      rating4: 4,
    };

    cy.request({
      method: "PUT",
      url: `${baseUrl}/api/review/${reviewId}`,
      body,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("PUT /api/review/{id} - Deve atualizar review com autor autenticado", function () {
    const body = {
      title: "Restaurante Bom",
      comment: "Review editada via Cypress",
      rating1: 4,
      rating2: 4,
      rating3: 4,
      rating4: 3,
    };

    cy.request({
      method: "PUT",
      url: `${baseUrl}/api/review/${reviewId}`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body,
    }).then((response) => {
      expect(response.status).to.equal(200);
    });
  });

  it("POST /api/review/{id}/upload-image - Deve negar upload sem token", function () {
    cy.request({
      method: "POST",
      url: `${baseUrl}/api/review/${reviewId}/upload-image`,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("POST /api/review/{id}/upload-image - Deve aceitar requisição com autor autenticado (se multipart configurado)", function () {
    cy.request({
      method: "POST",
      url: `${baseUrl}/api/review/${reviewId}/upload-image`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
      failOnStatusCode: false,
    }).then((response) => {
      expect([200, 400, 415]).to.include(response.status);
    });
  });

  it("DELETE /api/review/{id} - Deve negar exclusão sem token", function () {
    cy.request({
      method: "DELETE",
      url: `${baseUrl}/api/review/${reviewId}`,
      failOnStatusCode: false,
    }).then((response) => {
      expect([401, 403]).to.include(response.status);
    });
  });

  it("DELETE /api/review/{id} - Deve excluir review com autor autenticado", function () {
    cy.request({
      method: "DELETE",
      url: `${baseUrl}/api/review/${reviewId}`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }).then((response) => {
      expect(response.status).to.equal(204);
    });
  });
});
