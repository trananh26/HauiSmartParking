using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Auto_parking
{
    class FindContours
    {
        public int count = 0;

        /// <summary>
        /// Identifies rectangular contours in the specified color image using adaptive thresholding and contour analysis.
        /// </summary>
        /// <param name="colorImage"></param>
        /// <param name="thresholdValue"></param>
        /// <param name="invert"></param>
        /// <param name="processedGray"></param>
        /// <param name="processedColor"></param>
        /// <param name="listRectangles"></param>
        /// <returns>The number of contours detected that meet the specified criteria.</returns>
        public int IdentifyContours(Bitmap colorImage,
            int thresholdValue,
            bool invert,
            out Bitmap processedGray,
            out Bitmap processedColor,
            out List<Rectangle> listRectangles)
        {
            listRectangles = new List<Rectangle>();
            Image<Gray, byte> inputGrayImage = null;
            Image<Gray, byte> binaryImage = null;
            Image<Bgr, byte> inputColorImage = null;
            Image<Gray, byte> bestBinaryImage = null;
            Image<Gray, byte> bestFilledImage = null;
            Image<Bgr, byte> bestColorImage = null;

            try
            {
                // ========== BƯỚC 1: CHUẨN BỊ DỮ LIỆU BAN ĐẦU ==========
                // Chuyển ảnh màu thành ảnh xám để xử lý dễ dàng hơn
                inputGrayImage = colorImage.ToGrayImage();

                // Tạo ảnh nhị phân (chỉ có 2 tones: đen/trắng) để lưu kết quả
                binaryImage = new Image<Gray, byte>(inputGrayImage.Width, inputGrayImage.Height);

                // Tạo ảnh BGR để vẽ contour lên (kết quả hiển thị)
                inputColorImage = colorImage.ToBgrImage();

                // Mảng lưu trữ 8 contour hình chữ nhật tốt nhất
                Rectangle[] bestRectangles = new Rectangle[9];

                // Khởi tạo các ảnh "tốt nhất" hiện tại
                bestColorImage = colorImage.ToBgrImage();
                bestBinaryImage = inputGrayImage.Clone();
                bestFilledImage = binaryImage.Clone();

                int bestContourCount = 0;

                // ========== BƯỚC 2: VÒNG LẶP THỬ NHIỀU NGƯỎNG KHÁC NHAU ==========
                // Thử các giá trị ngưỡng: 126, 128, 130, 131, 133, 134, ...
                // Mục đích: tìm ngưỡng cho phát hiện biển số chính xác nhất
                for (double thresholdOffset = 0; thresholdOffset <= 127; thresholdOffset += 3)
                {
                    // Hệ số thresholdSign điều chỉnh: -1 (126, 131, ...) hoặc +1 (128, 130, ...)
                    for (int thresholdSign = -1; thresholdSign <= 1 && thresholdSign + thresholdOffset != 1; thresholdSign += 2)
                    {
                        Image<Bgr, byte> currentColorImage = null;
                        Image<Gray, byte> currentBinaryImage = null;
                        Image<Gray, byte> currentThresholdImage = null;

                        try
                        {
                            // Tạo các ảnh tạm thời để thử ngưỡng hiện tại
                            currentColorImage = colorImage.ToBgrImage();
                            currentBinaryImage = binaryImage.Clone();
                            listRectangles.Clear();
                            int currentContourCount = 0;

                            // Tính ngưỡng thực tế: currentThreshold = 127 + thresholdOffset * thresholdSign
                            double currentThreshold = 127 + thresholdOffset * thresholdSign;

                            // ========== BƯỚC 3: NHỊ PHÂN HÓA VÀ TÌM CONTOUR ==========
                            // Chuyển ảnh xám thành nhị phân dựa trên ngưỡng currentThreshold
                            // Pixel > currentThreshold → trắng (255), pixel <= currentThreshold → đen (0)
                            currentThresholdImage = inputGrayImage.ThresholdBinary(new Gray(currentThreshold), new Gray(255));

                            // Tìm tất cả contour (đường viền) trong ảnh nhị phân
                            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                            {
                                CvInvoke.FindContours(currentThresholdImage, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

                                for (int i = 0; i < contours.Size; i++)
                                {
                                    using (VectorOfPoint contour = contours[i])
                                    {
                                        // Lấy hình chữ nhật bao quanh contour
                                        Rectangle rect = CvInvoke.BoundingRectangle(contour);

                                        // Vẽ tất cả contour bằng màu vàng (255, 255, 0)
                                        CvInvoke.DrawContours(currentColorImage, contours, i, new MCvScalar(255, 255, 0), 1);

                                        // ========== BƯỚC 4: LỌC CONTOUR HỢP LỆ (BIỂN SỐ) ==========
                                        // Tính tỷ lệ chiều rộng/chiều cao
                                        double ratio = (double)rect.Width / rect.Height;

                                        // Kiểm tra các tiêu chí của biển số xe:
                                        // - Chiều rộng: 20-150 pixel
                                        // - Chiều cao: 80-180 pixel
                                        // - Tỷ lệ w/h: 0.1-1.1 (tránh quá hẹp/quá rộng)
                                        // - Vị trí X: > 20 pixel (tránh lề trái)
                                        if (rect.Width > 20 && rect.Width < 150
                                                && rect.Height > 80 && rect.Height < 180
                                      && ratio > 0.1 && ratio < 1.1 && rect.X > 20)
                                        {
                                            currentContourCount++;

                                            // Vẽ contour hợp lệ bằng màu cyan (0, 255, 255) - đậm hơn
                                            CvInvoke.DrawContours(currentColorImage, contours, i, new MCvScalar(0, 255, 255), 3);

                                            // Vẽ hình chữ nhật màu xanh lá
                                            currentColorImage.Draw(rect, new Bgr(Color.Green), 2);

                                            // Tô kín contour trong ảnh nhị phân (màu trắng)
                                            CvInvoke.DrawContours(currentBinaryImage, contours, i, new MCvScalar(255), -1);

                                            // Thêm vào danh sách hình chữ nhật
                                            listRectangles.Add(rect);
                                        }
                                    }
                                }
                            }

                            // ========== BƯỚC 5: LOẠI BỎ CONTOUR CHỒNG LẬP ==========
                            // Kiểm tra và xóa những hình chữ nhật trùng nhau
                            double averageHeight = 0;
                            double heightDeviation = 0;
                            for (int i = 0; i < currentContourCount; i++)
                            {
                                averageHeight += listRectangles[i].Height;
                                for (int j = i + 1; j < currentContourCount; j++)
                                {
                                    // Kiểm tra nếu hình chữ nhật j chồng với hình i
                                    if ((listRectangles[j].X < (listRectangles[i].X + listRectangles[i].Width) && listRectangles[j].X > listRectangles[i].X)
                              && (listRectangles[j].Y < (listRectangles[i].Y + listRectangles[i].Width) && listRectangles[j].Y > listRectangles[i].Y))
                                    {
                                        listRectangles.RemoveAt(j);
                                        currentContourCount--;
                                        j--;
                                    }
                                    // Kiểm tra nếu hình chữ nhật i chồng với hình j
                                    else if ((listRectangles[i].X < (listRectangles[j].X + listRectangles[j].Width) && listRectangles[i].X > listRectangles[j].X)
                                       && (listRectangles[i].Y < (listRectangles[j].Y + listRectangles[j].Width) && listRectangles[i].Y > listRectangles[j].Y))
                                    {
                                        averageHeight -= listRectangles[i].Height;
                                        listRectangles.RemoveAt(i);
                                        currentContourCount--;
                                        i--;
                                        break;
                                    }
                                }
                            }

                            // ========== BƯỚC 6: TÍNH ĐỘ ĐỀU ĐẶN CHIỀU CAO ==========
                            // Tính chiều cao trung bình của các biển số
                            if (currentContourCount > 0)
                            {
                                averageHeight = averageHeight / currentContourCount;

                                // Tính tổng độ lệch chiều cao (độ đều đặn)
                                // Nếu heightDeviation nhỏ → chiều cao đều đặn (tốt)
                                for (int i = 0; i < currentContourCount; i++)
                                {
                                    heightDeviation += Math.Abs(averageHeight - listRectangles[i].Height);
                                }
                            }

                            // ========== BƯỚC 7: LƯU KẾT QUẢ TỐT NHẤT ==========
                            // Kiểm tra nếu kết quả hiện tại tốt hơn kết quả trước đó
                            // Điều kiện:
                            // - Số biển số: 2-8 (currentContourCount <= 8 && currentContourCount > 1)
                            // - Nhiều hơn kết quả trước (currentContourCount > bestContourCount)
                            // - Chiều cao đều đặn (heightDeviation <= currentContourCount * 8)
                            if (currentContourCount <= 8 && currentContourCount > 1 && currentContourCount > bestContourCount && heightDeviation <= currentContourCount * 8)
                            {
                                // Sao chép danh sách hình chữ nhật tốt nhất
                                listRectangles.CopyTo(bestRectangles);
                                bestContourCount = currentContourCount;

                                // Giải phóng các ảnh "tốt nhất" cũ
                                if (bestColorImage != null) bestColorImage.Dispose();
                                if (bestFilledImage != null) bestFilledImage.Dispose();
                                if (bestBinaryImage != null) bestBinaryImage.Dispose();

                                // Lưu ảnh hiện tại làm ảnh "tốt nhất"
                                bestColorImage = currentColorImage;
                                bestFilledImage = currentBinaryImage;
                                bestBinaryImage = currentThresholdImage;

                                // Đặt thành null để không bị xóa trong finally block
                                currentColorImage = null;
                                currentBinaryImage = null;
                                currentThresholdImage = null;
                            }
                        }
                        finally
                        {
                            // Giải phóng ảnh tạm thời (nếu không được lưu làm tốt nhất)
                            if (currentColorImage != null) currentColorImage.Dispose();
                            if (currentBinaryImage != null) currentBinaryImage.Dispose();
                            if (currentThresholdImage != null) currentThresholdImage.Dispose();
                        }
                    }
                    // Dừng nếu đã tìm được đủ 8 biển số
                    if (bestContourCount == 8) break;
                }

                count = bestContourCount;

                // ========== BƯỚC 8: TRẢ KẾT QUẢ ==========
                // Tạo danh sách hình chữ nhật cuối cùng (loại bỏ những hình rỗng)
                listRectangles.Clear();
                for (int i = 0; i < bestRectangles.Length; i++)
                {
                    if (bestRectangles[i].Height != 0) listRectangles.Add(bestRectangles[i]);
                }

                // Chuyển các ảnh Emgu.CV sang Bitmap để trả về
                processedColor = bestColorImage.ToBitmap();
                processedGray = bestBinaryImage.ToBitmap();

                // Giải phóng các ảnh Emgu.CV sau khi chuyển đổi
                bestColorImage.Dispose();
                bestBinaryImage.Dispose();
                bestFilledImage.Dispose();

                // Đặt thành null để tránh giải phóng hai lần trong finally
                bestColorImage = null;
                bestBinaryImage = null;
                bestFilledImage = null;
            }
            finally
            {
                // Dọn dẹp tất cả các ảnh Emgu.CV còn lại chưa được giải phóng
                if (inputGrayImage != null) inputGrayImage.Dispose();
                if (binaryImage != null) binaryImage.Dispose();
                if (inputColorImage != null) inputColorImage.Dispose();
                if (bestBinaryImage != null) bestBinaryImage.Dispose();
                if (bestFilledImage != null) bestFilledImage.Dispose();
                if (bestColorImage != null) bestColorImage.Dispose();
            }

            return count;
        }
    }
}
