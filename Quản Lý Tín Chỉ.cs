using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quản_Lý_Đăng_Ký_Tín_Chỉ
{
    public partial class frmSinhVien : Form
    {
        // Chuỗi kết nối SQL Server (Chỉnh sửa lại chuỗi theo máy của bạn nếu cần)
        private string connectionString = @"Data Source=DESKTOP-13A3T4U\SQLEXPRESS;Initial Catalog=QL_TinChi;Integrated Security=True";

        public frmSinhVien()
        {
            InitializeComponent();
        }

        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            LoadComboBoxLop();

            // Thiết lập giá trị mặc định cho ComboBox Giới tính nếu có
            if (cboGioiTinh.Items.Count > 0)
                cboGioiTinh.SelectedIndex = 0;

            LoadDataSinhVien();
        }

        // 1. Tải danh sách Lớp vào ComboBox cboLop
        private void LoadComboBoxLop()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT Malop, Tenlop FROM LOP";
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cboLop.DataSource = dt;
                    cboLop.DisplayMember = "Tenlop";
                    cboLop.ValueMember = "Malop";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải danh sách lớp: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 2. Tải danh sách Sinh viên hiển thị lên DataGridView
        private void LoadDataSinhVien()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT Masv, Hoten, Ngaysinh, Gioitinh, Malop, Diachi, Sdt FROM SINHVIEN";
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvSinhVien.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải dữ liệu sinh viên: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 3. Nút LƯU (Thêm mới sinh viên vào CSDL)
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text) || string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã SV và Họ tên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = @"INSERT INTO SINHVIEN (Masv, Hoten, Ngaysinh, Gioitinh, Diachi, Sdt, Malop) 
                                 VALUES (@Masv, @Hoten, @Ngaysinh, @Gioitinh, @Diachi, @Sdt, @Malop)";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Masv", txtMaSV.Text.Trim());
                    cmd.Parameters.AddWithValue("@Hoten", txtHoTen.Text.Trim());
                    cmd.Parameters.AddWithValue("@Ngaysinh", dtpNgaySinh.Value);
                    cmd.Parameters.AddWithValue("@Gioitinh", cboGioiTinh.Text);
                    cmd.Parameters.AddWithValue("@Diachi", txtDiaChi.Text.Trim());
                    cmd.Parameters.AddWithValue("@Sdt", txtDienThoai.Text.Trim());
                    cmd.Parameters.AddWithValue("@Malop", cboLop.SelectedValue != null ? cboLop.SelectedValue.ToString() : "");

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thêm mới sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadDataSinhVien();
                    btnLamMoiForm_Click(sender, e); // Xóa trắng form sau khi lưu
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm sinh viên (có thể trùng Mã SV): " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 4. Nút SỬA (Cập nhật thông tin sinh viên)
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần sửa từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = @"UPDATE SINHVIEN 
                                 SET Hoten = @Hoten, Ngaysinh = @Ngaysinh, Gioitinh = @Gioitinh, 
                                     Diachi = @Diachi, Sdt = @Sdt, Malop = @Malop 
                                 WHERE Masv = @Masv";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Masv", txtMaSV.Text.Trim());
                    cmd.Parameters.AddWithValue("@Hoten", txtHoTen.Text.Trim());
                    cmd.Parameters.AddWithValue("@Ngaysinh", dtpNgaySinh.Value);
                    cmd.Parameters.AddWithValue("@Gioitinh", cboGioiTinh.Text);
                    cmd.Parameters.AddWithValue("@Diachi", txtDiaChi.Text.Trim());
                    cmd.Parameters.AddWithValue("@Sdt", txtDienThoai.Text.Trim());
                    cmd.Parameters.AddWithValue("@Malop", cboLop.SelectedValue != null ? cboLop.SelectedValue.ToString() : "");

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cập nhật thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataSinhVien();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi sửa dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 5. Nút XÓA (Xóa sinh viên khỏi CSDL)
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên " + txtMaSV.Text + " không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string sql = "DELETE FROM SINHVIEN WHERE Masv = @Masv";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@Masv", txtMaSV.Text.Trim());

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadDataSinhVien();
                        btnLamMoiForm_Click(sender, e);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // 6. Nút LÀM MỚI FORM (Xóa trắng thông tin nhập liệu)
        private void btnLamMoiForm_Click(object sender, EventArgs e)
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            txtDiaChi.Clear();
            txtDienThoai.Clear();
            dtpNgaySinh.Value = DateTime.Now;

            if (cboGioiTinh.Items.Count > 0) cboGioiTinh.SelectedIndex = 0;
            if (cboLop.Items.Count > 0) cboLop.SelectedIndex = 0;

            txtMaSV.Focus();
        }

        // 7. Nút THOÁT
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 8. Nút TÌM KIẾM (Tìm theo Mã SV, Họ tên, Giới tính, hoặc SĐT)
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = @"SELECT Masv, Hoten, Ngaysinh, Gioitinh, Malop, Diachi, Sdt 
                                 FROM SINHVIEN 
                                 WHERE (@Masv = '' OR Masv LIKE '%' + @Masv + '%')
                                   AND (@Hoten = '' OR Hoten LIKE '%' + @Hoten + '%')
                                   AND (@Gioitinh = '' OR Gioitinh LIKE '%' + @Gioitinh + '%')
                                   AND (@Sdt = '' OR Sdt LIKE '%' + @Sdt + '%')";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Masv", txtTK_MaSV.Text.Trim());
                    cmd.Parameters.AddWithValue("@Hoten", txtTK_HoTen.Text.Trim());
                    cmd.Parameters.AddWithValue("@Gioitinh", cboTK_GioiTinh.Text.Trim());
                    cmd.Parameters.AddWithValue("@Sdt", txtTK_DienThoai.Text.Trim());

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvSinhVien.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy sinh viên nào phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 9. Nút LÀM MỚI DANH SÁCH (Hủy lọc tìm kiếm, tải lại toàn bộ)
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTK_MaSV.Clear();
            txtTK_HoTen.Clear();
            txtTK_DienThoai.Clear();
            cboTK_GioiTinh.SelectedIndex = -1;

            LoadDataSinhVien();
        }

        // 10. Sự kiện Click dòng trên DataGridView
        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSinhVien.Rows[e.RowIndex];

                txtMaSV.Text = row.Cells["Masv"].Value?.ToString();
                txtHoTen.Text = row.Cells["Hoten"].Value?.ToString();

                if (row.Cells["Ngaysinh"].Value != DBNull.Value && row.Cells["Ngaysinh"].Value != null)
                    dtpNgaySinh.Value = Convert.ToDateTime(row.Cells["Ngaysinh"].Value);

                cboGioiTinh.Text = row.Cells["Gioitinh"].Value?.ToString();
                cboLop.SelectedValue = row.Cells["Malop"].Value?.ToString();
                txtDiaChi.Text = row.Cells["Diachi"].Value?.ToString();
                txtDienThoai.Text = row.Cells["Sdt"].Value?.ToString();
            }
        }
    }
}