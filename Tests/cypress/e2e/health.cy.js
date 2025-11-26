describe("API Health", () => {
  const baseUrl = "http://localhost:5000";

  it("GET /api/health - Deve retornar OK e estar pública", () => {
    cy.request("GET", `${baseUrl}/api/health`).then((response) => {
      expect(response.status).to.equal(200);
    });
  });
});
