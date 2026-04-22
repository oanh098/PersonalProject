using System;

namespace PersonalProject.Models;

public class SeepayNotification
{
    public string Gateway { get; set; } = string.Empty;
    public string TransactionDate { get; set; } = string.Empty; // Example: "2024-06-01T12:34:56Z"
    public string AccountNumber { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // This is the "Order ID" that you sent in the "content" field of the payment request
    public string TransferType { get; set; } = string.Empty; // Example: "IN" for incoming payment
    public decimal TransferAmount { get; set; } = 0; // This maps to "transferAmount": 5000
    public string ReferenceCode { get; set; } = string.Empty;
    public long Id { get; set; } = 0;
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