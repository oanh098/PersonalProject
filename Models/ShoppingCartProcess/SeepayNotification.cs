using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalProject.Models.ShoppingCartProcess;

public class SeepayNotification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("gateway")]
    [StringLength(100)]
    public string Gateway { get; set; } = string.Empty;

    [Required]
    [Column("transaction_date")]
    public string TransactionDate { get; set; } = string.Empty; // Example: "2024-06-01T12:34:56Z"

    [Column("account_number")]
    [StringLength(100)]    
    public string AccountNumber { get; set; } = string.Empty;

    [Column("sub_account")]
    [StringLength(250)]
    public string? SubAccount { get; set; }

    [Required]
    [Column("amount_in", TypeName = "decimal(20,2)")]
    public decimal AmountIn { get; set; } = 0.00m;

    [Required]
    [Column("amount_out", TypeName = "decimal(20,2)")]
    public decimal AmountOut { get; set; } = 0.00m;

    [Required]
    [Column("accumulated", TypeName = "decimal(20,2)")]
    public decimal Accumulated { get; set; } = 0.00m;

    [Column("code")]
    [StringLength(250)]
    public string? Code { get; set; }

    [Column("transaction_content")]
    public string? TransactionContent { get; set; }

    [Column("reference_number")]
    [StringLength(255)]
    public string? ReferenceNumber { get; set; }

    [Column("body")]
    public string? Body { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;    
}


// Cấu trúc sepay request và một mẫu data giao dịch thật.

// {
//     "id": 92704,                              // ID giao dịch trên SePay
//     "gateway":"Vietcombank",                  // Brand name của ngân hàng
//     "transactionDate":"2023-03-25 14:02:37",  // Thời gian xảy ra giao dịch phía ngân hàng
//     "accountNumber":"0123499999",              // Số tài khoản ngân hàng
//     "code":null,                               // Mã code thanh toán (sepay tự nhận diện dựa vào cấu hình tại Công ty -> Cấu hình chung)
//     "content":"chuyen tien mua iphone",        // Nội dung chuyển khoản
//     "transferType":"in",                       // Loại giao dịch. in là tiền vào, out là tiền ra
//     "transferAmount":2277000,                  // Số tiền giao dịch
//     "accumulated":19077000,                    // Số dư tài khoản (lũy kế)
//     "subAccount":null,                         // Tài khoản ngân hàng phụ (tài khoản định danh),
//     "referenceCode":"MBVCB.3278907687",         // Mã tham chiếu của tin nhắn sms
//     "description":""                           // Toàn bộ nội dung tin nhắn sms
// }


// {
//   "gateway": "Sacombank",
//   "transactionDate": "2026-04-16 14:45:26",
//   "accountNumber": "0853833045",
//   "subAccount": null,
//   "code": null,
//   "content": "MBVCB.13818942280.6106BFTVG29DK69 E.FJ0.CT tu 0531002582398 TRAN THUY OANH toi 0853833045 TRAN THUY OANH tai Sacombank CKN 043147 G29DK69E - TRAN THUY OANH - Ngan hang TMCP Ngoai thuong Viet Nam",
//   "transferType": "in",
//   "description": "BankAPINotify MBVCB.13818942280.6106BFTVG29DK69 E.FJ0.CT tu 0531002582398 TRAN THUY OANH toi 0853833045 TRAN THUY OANH tai Sacombank CKN 043147 G29DK69E - TRAN THUY OANH - Ngan hang TMCP Ngoai thuong Viet Nam",
//   "transferAmount": 5000,
//   "referenceCode": "VN0014171FT26106RWYQC",
//   "accumulated": 0,
//   "id": 51046939
// }