namespace KitchenBattle.Models
{
    public enum StatusBattleEnum
    {
        Pending, // Ожидает начала регистрации
        InProgress, // Идет регистрация или битва
        Completed, // Завершена
        Closed // Закрыта для регистрации и голосования
    }
}
