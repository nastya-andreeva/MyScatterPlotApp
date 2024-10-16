<?php
header('Content-Type: application/json');

$servername = "localhost";
$username = "root";
$password = "";
$dbname = "scatterplot_db";

// Создание соединения
$conn = new mysqli($servername, $username, $password, $dbname);

// Проверка соединения
if ($conn->connect_error) {
    echo json_encode(["status" => "error", "message" => "Connection failed: " . $conn->connect_error]);
    exit;
}

// Получение данных JSON
$data = json_decode(file_get_contents('php://input'), true);

if (!$data) {
    echo json_encode(["status" => "error", "message" => "No data received"]);
    exit;
}

$chartId = $data['chartId'];
$xValues = explode(",", $data['xValues']);
$yValues = explode(",", $data['yValues']);

// Генерация диаграммы с использованием библиотеки GD
$width = 800;
$height = 600;
$image = imagecreatetruecolor($width, $height);

// Цвета
$white = imagecolorallocate($image, 255, 255, 255);
$black = imagecolorallocate($image, 0, 0, 0);
$red = imagecolorallocate($image, 255, 0, 0);

// Заполнение фона белым
imagefilledrectangle($image, 0, 0, $width, $height, $white);

// Определение масштабов
$maxX = max($xValues);
$maxY = max($yValues);
$padding = 50;

// Рисование осей
imageline($image, $padding, $height - $padding, $padding, $padding, $black);
imageline($image, $padding, $height - $padding, $width - $padding, $height - $padding, $black);

// Рисование точек
for ($i = 0; $i < count($xValues); $i++) {
    $x = $padding + (($xValues[$i] / $maxX) * ($width - 2 * $padding));
    $y = ($height - $padding) - (($yValues[$i] / $maxY) * ($height - 2 * $padding));
    imagefilledellipse($image, $x, $y, 10, 10, $red);
}

// Сохранение изображения
$imagePath = "img/chart_$chartId.png";
if (!file_exists("img")) {
    mkdir("img", 0777, true);
}
imagepng($image, __DIR__ . "/$imagePath");
imagedestroy($image);

// Возврат пути к изображению
echo json_encode(["status" => "success", "imagePath" => "/php/$imagePath"]);
?>
