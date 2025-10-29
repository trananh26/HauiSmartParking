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
        public int IdentifyContours(
            Bitmap colorImage,
            int thresholdValue,
            bool invert,
            out Bitmap processedGray,
            out Bitmap processedColor,
            out List<Rectangle> listRectangles)
        {
            listRectangles = new List<Rectangle>();
            Image<Gray, byte> grayImage = null;
            Image<Gray, byte> bi = null;
            Image<Bgr, byte> color = null;
            Image<Gray, byte> src_b = null;
            Image<Gray, byte> bi_b = null;
            Image<Bgr, byte> color_b = null;

            try
            {
                // ========== BƯỚC 1: CHUẨN BỊ DỮ LIỆU BAN ĐẦU ==========
                // Chuyển ảnh màu thành ảnh xám để xử lý dễ dàng hơn
                grayImage = colorImage.ToGrayImage();

                // Tạo ảnh nhị phân (chỉ có 2 tones: đen/trắng) để lưu kết quả
                bi = new Image<Gray, byte>(grayImage.Width, grayImage.Height);

                // Tạo ảnh BGR để vẽ contour lên (kết quả hiển thị)
                color = colorImage.ToBgrImage();

                // Mảng lưu trữ 8 contour hình chữ nhật tốt nhất
                Rectangle[] li = new Rectangle[9];

                // Khởi tạo các ảnh "tốt nhất" hiện tại
                color_b = colorImage.ToBgrImage();
                src_b = grayImage.Clone();
                bi_b = bi.Clone();

                int c_best = 0;

                // ========== BƯỚC 2: VÒNG LẶP THỬ NHIỀU NGƯỎNG KHÁC NHAU ==========
                // Thử các giá trị ngưỡng: 126, 128, 130, 131, 133, 134, ...
                // Mục đích: tìm ngưỡng cho phát hiện biển số chính xác nhất
                for (double value = 0; value <= 127; value += 3)
                {
                    // Hệ số s điều chỉnh: -1 (126, 131, ...) hoặc +1 (128, 130, ...)
                    for (int s = -1; s <= 1 && s + value != 1; s += 2)
                    {
                        Image<Bgr, byte> color2 = null;
                        Image<Gray, byte> bi2 = null;
                        Image<Gray, byte> src = null;

                        try
                        {
                            // Tạo các ảnh tạm thời để thử ngưỡng hiện tại
                            color2 = colorImage.ToBgrImage();
                            bi2 = bi.Clone();
                            listRectangles.Clear();
                            int c = 0;

                            // Tính ngưỡng thực tế: t = 127 + value * s
                            double t = 127 + value * s;

                            // ========== BƯỚC 3: NHỊ PHÂN HÓA VÀ TÌM CONTOUR ==========
                            // Chuyển ảnh xám thành nhị phân dựa trên ngưỡng t
                            // Pixel > t → trắng (255), pixel <= t → đen (0)
                            src = grayImage.ThresholdBinary(new Gray(t), new Gray(255));

                            // Tìm tất cả contour (đường viền) trong ảnh nhị phân
                            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                            {
                                CvInvoke.FindContours(src, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

                                for (int i = 0; i < contours.Size; i++)
                                {
                                    using (VectorOfPoint contour = contours[i])
                                    {
                                        // Lấy hình chữ nhật bao quanh contour
                                        Rectangle rect = CvInvoke.BoundingRectangle(contour);

                                        // Vẽ tất cả contour bằng màu vàng (255, 255, 0)
                                        CvInvoke.DrawContours(color2, contours, i, new MCvScalar(255, 255, 0), 1);

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
                                            c++;

                                            // Vẽ contour hợp lệ bằng màu cyan (0, 255, 255) - đậm hơn
                                            CvInvoke.DrawContours(color2, contours, i, new MCvScalar(0, 255, 255), 3);

                                            // Vẽ hình chữ nhật màu xanh lá
                                            color2.Draw(rect, new Bgr(Color.Green), 2);

                                            // Tô kín contour trong ảnh nhị phân (màu trắng)
                                            CvInvoke.DrawContours(bi2, contours, i, new MCvScalar(255), -1);

                                            // Thêm vào danh sách hình chữ nhật
                                            listRectangles.Add(rect);
                                        }
                                    }
                                }
                            }

                            // ========== BƯỚC 5: LOẠI BỎ CONTOUR CHỒNG LẬP ==========
                            // Kiểm tra và xóa những hình chữ nhật trùng nhau
                            double avg_h = 0;
                            double dis = 0;
                            for (int i = 0; i < c; i++)
                            {
                                avg_h += listRectangles[i].Height;
                                for (int j = i + 1; j < c; j++)
                                {
                                    // Kiểm tra nếu hình chữ nhật j chồng với hình i
                                    if ((listRectangles[j].X < (listRectangles[i].X + listRectangles[i].Width) && listRectangles[j].X > listRectangles[i].X)
                                 && (listRectangles[j].Y < (listRectangles[i].Y + listRectangles[i].Width) && listRectangles[j].Y > listRectangles[i].Y))
                                    {
                                        listRectangles.RemoveAt(j);
                                        c--;
                                        j--;
                                    }
                                    // Kiểm tra nếu hình chữ nhật i chồng với hình j
                                    else if ((listRectangles[i].X < (listRectangles[j].X + listRectangles[j].Width) && listRectangles[i].X > listRectangles[j].X)
                                              && (listRectangles[i].Y < (listRectangles[j].Y + listRectangles[j].Width) && listRectangles[i].Y > listRectangles[j].Y))
                                    {
                                        avg_h -= listRectangles[i].Height;
                                        listRectangles.RemoveAt(i);
                                        c--;
                                        i--;
                                        break;
                                    }
                                }
                            }

                            // ========== BƯỚC 6: TÍNH ĐỘ ĐỀU ĐẶN CHIỀU CAO ==========
                            // Tính chiều cao trung bình của các biển số
                            if (c > 0)
                            {
                                avg_h = avg_h / c;

                                // Tính tổng độ lệch chiều cao (độ đều đặn)
                                // Nếu dis nhỏ → chiều cao đều đặn (tốt)
                                for (int i = 0; i < c; i++)
                                {
                                    dis += Math.Abs(avg_h - listRectangles[i].Height);
                                }
                            }

                            // ========== BƯỚC 7: LƯU KẾT QUẢ TỐT NHẤT ==========
                            // Kiểm tra nếu kết quả hiện tại tốt hơn kết quả trước đó
                            // Điều kiện:
                            // - Số biển số: 2-8 (c <= 8 && c > 1)
                            // - Nhiều hơn kết quả trước (c > c_best)
                            // - Chiều cao đều đặn (dis <= c * 8)
                            if (c <= 8 && c > 1 && c > c_best && dis <= c * 8)
                            {
                                // Sao chép danh sách hình chữ nhật tốt nhất
                                listRectangles.CopyTo(li);
                                c_best = c;

                                // Giải phóng các ảnh "tốt nhất" cũ
                                if (color_b != null) color_b.Dispose();
                                if (bi_b != null) bi_b.Dispose();
                                if (src_b != null) src_b.Dispose();

                                // Lưu ảnh hiện tại làm ảnh "tốt nhất"
                                color_b = color2;
                                bi_b = bi2;
                                src_b = src;

                                // Đặt thành null để không bị xóa trong finally block
                                color2 = null;
                                bi2 = null;
                                src = null;
                            }
                        }
                        finally
                        {
                            // Giải phóng ảnh tạm thời (nếu không được lưu làm tốt nhất)
                            if (color2 != null) color2.Dispose();
                            if (bi2 != null) bi2.Dispose();
                            if (src != null) src.Dispose();
                        }
                    }
                    // Dừng nếu đã tìm được đủ 8 biển số
                    if (c_best == 8) break;
                }

                count = c_best;

                // ========== BƯỚC 8: TRẢ KẾT QUẢ ==========
                // Tạo danh sách hình chữ nhật cuối cùng (loại bỏ những hình rỗng)
                listRectangles.Clear();
                for (int i = 0; i < li.Length; i++)
                {
                    if (li[i].Height != 0) listRectangles.Add(li[i]);
                }

                // Chuyển các ảnh Emgu.CV sang Bitmap để trả về
                processedColor = color_b.ToBitmap();
                processedGray = src_b.ToBitmap();

                // Giải phóng các ảnh Emgu.CV sau khi chuyển đổi
                color_b.Dispose();
                src_b.Dispose();
                bi_b.Dispose();

                // Đặt thành null để tránh giải phóng hai lần trong finally
                color_b = null;
                src_b = null;
                bi_b = null;
            }
            finally
            {
                // Dọn dẹp tất cả các ảnh Emgu.CV còn lại chưa được giải phóng
                if (grayImage != null) grayImage.Dispose();
                if (bi != null) bi.Dispose();
                if (color != null) color.Dispose();
                if (src_b != null) src_b.Dispose();
                if (bi_b != null) bi_b.Dispose();
                if (color_b != null) color_b.Dispose();
            }

            return count;
        }

    }
}
