// API Base URL
const API_BASE = '/api';

// Load data on page load
document.addEventListener('DOMContentLoaded', () => {
    carregarDashboard();
    carregarProdutos();
});

// Utility Functions
function formatCurrency(value) {
    return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    }).format(value);
}

function formatDate(dateString) {
    return new Date(dateString).toLocaleDateString('pt-BR');
}

function calcularMargem(precoCusto, precoVenda) {
    if (precoCusto === 0) return 0;
    return ((precoVenda - precoCusto) / precoCusto * 100).toFixed(1);
}

function getEstoqueBadge(quantidade) {
    if (quantidade <= 5) return 'badge-low';
    if (quantidade <= 20) return 'badge-medium';
    return 'badge-high';
}

// Dashboard
async function carregarDashboard() {
    try {
        const response = await fetch(`${API_BASE}/dashboard`);
        const data = await response.json();

        document.getElementById('totalProdutos').textContent = data.totalProdutos || 0;
        document.getElementById('lucroTotal').textContent = formatCurrency(data.lucroTotal || 0);
        document.getElementById('totalEstoque').textContent = data.totalEstoque || 0;
        document.getElementById('valorEstoque').textContent = formatCurrency(data.valorEstoque || 0);
    } catch (error) {
        console.error('Erro ao carregar dashboard:', error);
    }
}

// Produtos
async function carregarProdutos() {
    try {
        const response = await fetch(`${API_BASE}/produtos`);
        const produtos = await response.json();

        const tbody = document.getElementById('produtosBody');
        
        if (produtos.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="empty-state">Nenhum produto cadastrado</td></tr>';
            return;
        }

        tbody.innerHTML = produtos.map(p => `
            <tr>
                <td>${p.id}</td>
                <td><strong>${p.nome}</strong></td>
                <td>
                    <span class="badge ${getEstoqueBadge(p.quantidadeEstoque)}">
                        ${p.quantidadeEstoque}
                    </span>
                </td>
                <td>${formatCurrency(p.precoCusto)}</td>
                <td>${formatCurrency(p.precoVenda)}</td>
                <td>${calcularMargem(p.precoCusto, p.precoVenda)}%</td>
                <td>
                    <button class="btn btn-sm btn-success" onclick="abrirModalTransacao(${p.id}, '${p.nome}')">
                        💰 Transação
                    </button>
                    <button class="btn btn-sm btn-primary" onclick="editarProduto(${p.id})">
                        ✏️ Editar
                    </button>
                    <button class="btn btn-sm btn-danger" onclick="excluirProduto(${p.id}, '${p.nome}')">
                        🗑️ Excluir
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (error) {
        console.error('Erro ao carregar produtos:', error);
        document.getElementById('produtosBody').innerHTML = 
            '<tr><td colspan="7" class="empty-state" style="color: var(--danger);">Erro ao carregar produtos</td></tr>';
    }
}

// Modal Functions
function abrirModal(modalId) {
    document.getElementById(modalId).classList.add('active');
}

function fecharModal(modalId) {
    document.getElementById(modalId).classList.remove('active');
}

function abrirModalNovoProduto() {
    document.getElementById('formProduto').reset();
    document.getElementById('produtoId').value = '';
    document.getElementById('modalProdutoTitle').textContent = 'Novo Produto';
    abrirModal('modalProduto');
}

async function editarProduto(id) {
    try {
        const response = await fetch(`${API_BASE}/produtos/${id}`);
        const produto = await response.json();

        document.getElementById('produtoId').value = produto.id;
        document.getElementById('nome').value = produto.nome;
        document.getElementById('quantidadeEstoque').value = produto.quantidadeEstoque;
        document.getElementById('precoCusto').value = produto.precoCusto;
        document.getElementById('precoVenda').value = produto.precoVenda;
        
        document.getElementById('modalProdutoTitle').textContent = 'Editar Produto';
        abrirModal('modalProduto');
    } catch (error) {
        alert('Erro ao carregar produto para edição');
    }
}

async function salvarProduto(event) {
    event.preventDefault();

    const id = document.getElementById('produtoId').value;
    const produto = {
        id: id ? parseInt(id) : 0,
        nome: document.getElementById('nome').value,
        quantidadeEstoque: parseInt(document.getElementById('quantidadeEstoque').value),
        precoCusto: parseFloat(document.getElementById('precoCusto').value),
        precoVenda: parseFloat(document.getElementById('precoVenda').value),
        dataCadastro: new Date().toISOString()
    };

    // Validação
    if (produto.precoVenda <= produto.precoCusto) {
        alert('O preço de venda deve ser maior que o preço de custo!');
        return;
    }

    try {
        const url = id ? `${API_BASE}/produtos/${id}` : `${API_BASE}/produtos`;
        const method = id ? 'PUT' : 'POST';

        const response = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(produto)
        });

        if (response.ok) {
            fecharModal('modalProduto');
            carregarProdutos();
            carregarDashboard();
            alert(id ? 'Produto atualizado com sucesso!' : 'Produto cadastrado com sucesso!');
        } else {
            alert('Erro ao salvar produto');
        }
    } catch (error) {
        alert('Erro ao salvar produto: ' + error.message);
    }
}

async function excluirProduto(id, nome) {
    if (!confirm(`Tem certeza que deseja excluir o produto "${nome}"?`)) {
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/produtos/${id}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            carregarProdutos();
            carregarDashboard();
            alert('Produto excluído com sucesso!');
        } else {
            alert('Erro ao excluir produto');
        }
    } catch (error) {
        alert('Erro ao excluir produto: ' + error.message);
    }
}

// Transações
function abrirModalTransacao(produtoId, produtoNome) {
    document.getElementById('formTransacao').reset();
    document.getElementById('transacaoProdutoId').value = produtoId;
    document.getElementById('transacaoProdutoNome').value = produtoNome;
    document.getElementById('modalTransacaoTitle').textContent = `Transação - ${produtoNome}`;
    abrirModal('modalTransacao');
}

async function registrarTransacao(event) {
    event.preventDefault();

    const transacao = {
        produtoId: parseInt(document.getElementById('transacaoProdutoId').value),
        nomeProduto: document.getElementById('transacaoProdutoNome').value,
        tipo: document.getElementById('tipoTransacao').value,
        quantidade: parseInt(document.getElementById('quantidadeTransacao').value),
        data: new Date().toISOString(),
        valorUnitario: 0, // Will be calculated on server
        lucroObtido: 0 // Will be calculated on server
    };

    try {
        const response = await fetch(`${API_BASE}/transacoes`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(transacao)
        });

        if (response.ok) {
            fecharModal('modalTransacao');
            carregarProdutos();
            carregarDashboard();
            alert('Transação registrada com sucesso!');
        } else {
            const error = await response.text();
            alert('Erro: ' + error);
        }
    } catch (error) {
        alert('Erro ao registrar transação: ' + error.message);
    }
}

// Close modal on outside click
window.onclick = function(event) {
    if (event.target.classList.contains('modal')) {
        event.target.classList.remove('active');
    }
}
