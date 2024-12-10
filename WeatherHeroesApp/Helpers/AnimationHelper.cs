using System.Collections.Generic;

namespace WeatherHeroesApp.Helpers
{
    public static class AnimationHelper
    {
        // Caminhos das animações, ícones dos heróis e cores de fundo
        private static readonly Dictionary<string, (string Animation, string Icon, string BackgroundColor, string Message)> WeatherAnimations =
    new()
    {
            { "Clear", ("clear.json", "flash.png", "#FFD700", "Bright and clear skies ahead! Stay energized like Flash.") },
            { "Thunderstorm", ("thunderr.json", "thor.png", "#1E90FF", "Thunder roars! Thor is showing his might. Stay safe.") },
            { "Snow", ("snowfall.json", "cyclops.png", "#E0FFFF", "Snowy and cold! Cyclops commands icy conditions.") },
            { "Rain", ("rain.json", "hellboy.png", "#4682B4", "Rain pouring down! Hellboy is bringing a storm.") },
            { "Clouds", ("clouds.json", "batman.png", "#B0C4DE", "Overcast and mysterious, just like Batman.") },
            { "Drizzle", ("drizzle.json", "hellboy.png", "#87CEFA", "Light drizzle falling gently. A calm and quiet moment.") },
            { "Fog", ("fog.json", "batman.png", "#708090", "Dense fog ahead. Batman moves through the shadows.") },
            { "Mist", ("fog.json", "batman.png", "#708090", "Misty and serene. Proceed with caution.") },
            { "Dust", ("dust.json", "eobard.png", "#DEB887", "Dusty winds swirl! The Reverse Flash is stirring chaos.") },
            { "Sand", ("dust.json", "eobard.png", "#DEB887", "Sandy and wild. Prepare for challenging conditions.") },
            { "Extreme", ("thunderr.json", "thor.png", "#FF4500", "Extreme weather conditions! Thor's power is unleashed.") },
            { "Default", ("clear.json", "flash.png", "#87CEFA", "Weather data unavailable. Stay ready and vigilant.") }
    };

        /// <summary>
        /// Retorna o caminho da animação com base na condição climática.
        /// </summary>
        public static string GetAnimation(string weatherCondition) =>
            WeatherAnimations.ContainsKey(weatherCondition)
                ? WeatherAnimations[weatherCondition].Animation
                : WeatherAnimations["Default"].Animation;

        /// <summary>
        /// Retorna o caminho do ícone (imagem) do herói com base na condição climática.
        /// </summary>
        public static string GetIcon(string weatherCondition) =>
            WeatherAnimations.ContainsKey(weatherCondition)
                ? WeatherAnimations[weatherCondition].Icon
                : WeatherAnimations["Default"].Icon;

        /// <summary>
        /// Retorna a cor do fundo com base na condição climática.
        /// </summary>
        public static string GetBackgroundColor(string weatherCondition) =>
            WeatherAnimations.ContainsKey(weatherCondition)
                ? WeatherAnimations[weatherCondition].BackgroundColor
                : WeatherAnimations["Default"].BackgroundColor;

        /// <summary>
        /// Retorna uma mensagem personalizada com base na condição climática.
        /// </summary>
        public static string GetMessage(string weatherCondition) =>
            WeatherAnimations.ContainsKey(weatherCondition)
                ? WeatherAnimations[weatherCondition].Message
                : WeatherAnimations["Default"].Message;
    }
}
