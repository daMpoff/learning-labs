"""
Реализация метода TOPSIS (Technique for Order of Preference by Similarity to Ideal Solution)
"""
import numpy as np
from typing import List, Dict, Tuple, Optional


def calculate_topsis(
    decision_matrix: List[List[float]],
    weights: List[float],
    criteria_types: List[str]
) -> Tuple[List[float], List[Dict]]:
    """
    Расчет TOPSIS для матрицы решений
    
    Args:
        decision_matrix: Матрица решений (альтернативы × критерии)
        weights: Веса критериев (должны суммироваться в 1.0)
        criteria_types: Типы критериев ('max' или 'min')
    
    Returns:
        Tuple[List[float], List[Dict]]: 
        - Список значений относительной близости (Ci) для каждой альтернативы
        - Список словарей с детальной информацией о результатах
    """
    if not decision_matrix:
        raise ValueError("Матрица решений не может быть пустой")
    
    matrix = np.array(decision_matrix, dtype=float)
    n_alternatives, n_criteria = matrix.shape
    
    if len(weights) != n_criteria:
        raise ValueError(f"Количество весов ({len(weights)}) не совпадает с количеством критериев ({n_criteria})")
    
    if len(criteria_types) != n_criteria:
        raise ValueError(f"Количество типов критериев ({len(criteria_types)}) не совпадает с количеством критериев ({n_criteria})")
    
    # Нормализация весов (сумма должна быть равна 1.0)
    weights_sum = sum(weights)
    if abs(weights_sum - 1.0) > 1e-6:
        weights = [w / weights_sum for w in weights]
    
    # Шаг 1: Векторная нормализация матрицы решений
    # Для каждого критерия: rij = xij / sqrt(sum(xij^2))
    normalized_matrix = np.zeros_like(matrix)
    for j in range(n_criteria):
        column = matrix[:, j]
        norm = np.sqrt(np.sum(column ** 2))
        if norm > 0:
            normalized_matrix[:, j] = column / norm
        else:
            normalized_matrix[:, j] = 0
    
    # Шаг 2: Взвешивание нормализованной матрицы
    # vij = wj * rij
    weighted_matrix = normalized_matrix * np.array(weights)
    
    # Шаг 3: Определение идеального положительного решения (PIS) и идеального отрицательного решения (NIS)
    # PIS: максимум для max-критериев, минимум для min-критериев
    # NIS: минимум для max-критериев, максимум для min-критериев
    pis = np.zeros(n_criteria)
    nis = np.zeros(n_criteria)
    
    for j in range(n_criteria):
        column = weighted_matrix[:, j]
        if criteria_types[j].lower() == 'max':
            pis[j] = np.max(column)
            nis[j] = np.min(column)
        else:  # 'min'
            pis[j] = np.min(column)
            nis[j] = np.max(column)
    
    # Шаг 4: Расчет расстояний до PIS и NIS (евклидово расстояние)
    # di+ = sqrt(sum((vij - vj+)^2))
    # di- = sqrt(sum((vij - vj-)^2))
    distances_pis = np.sqrt(np.sum((weighted_matrix - pis) ** 2, axis=1))
    distances_nis = np.sqrt(np.sum((weighted_matrix - nis) ** 2, axis=1))
    
    # Шаг 5: Расчет относительной близости к идеальному решению
    # Ci = di- / (di+ + di-)
    relative_closeness = distances_nis / (distances_pis + distances_nis)
    
    # Обработка случая, когда di+ + di- = 0 (все альтернативы одинаковы)
    relative_closeness = np.nan_to_num(relative_closeness, nan=0.0, posinf=1.0, neginf=0.0)
    
    # Формирование детальных результатов
    results = []
    for i in range(n_alternatives):
        results.append({
            'alternative_index': i,
            'distance_to_pis': float(distances_pis[i]),
            'distance_to_nis': float(distances_nis[i]),
            'relative_closeness': float(relative_closeness[i])
        })
    
    return relative_closeness.tolist(), results


def rank_alternatives(relative_closeness: List[float]) -> List[Tuple[int, float]]:
    """
    Ранжирование альтернатив по значениям относительной близости
    
    Args:
        relative_closeness: Список значений относительной близости
    
    Returns:
        List[Tuple[int, float]]: Список кортежей (индекс альтернативы, значение Ci), отсортированный по убыванию
    """
    ranked = [(i, ci) for i, ci in enumerate(relative_closeness)]
    ranked.sort(key=lambda x: x[1], reverse=True)
    return ranked
