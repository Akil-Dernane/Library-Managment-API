using Library_API.DTOs.Loan;
using Library_API.Exceptions.Book;
using Library_API.Exceptions.Loan;
using Library_API.Exceptions.Member;
using Library_API.Mappings;
using Library_API.Models;
using Library_API.Repositories.Interfaces;
using Library_API.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Library_API.Services
{
    public class LoanService : ILoanService
    {
        private readonly int _borrowingLimit;
        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;

        public LoanService(
            IOptions<LibrarySettings> options,
            ILoanRepository loanRepository,
            IBookRepository bookRepository,
            IMemberRepository memberRepository
            )
        {
            _borrowingLimit = options.Value.BorrowingLimit;
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _memberRepository = memberRepository; 
        }

        public async Task<LoanResponseDto> Create(CreateLoanDto dto)
        {
            Loan loan = dto.ToEntity();

            // check member existance + limit
            Member member = await _memberRepository.GetByIdWithRelationships(dto.MemberId) ?? throw new MemberNotFoundException(dto.MemberId);
            member.EnsureCanBorrow(_borrowingLimit);

            //check book existance + its availability
            Book book = await _bookRepository.GetById(dto.BookId) ?? throw new BookNotFoundException(dto.BookId);
            book.Borrow();

            loan.Book = book;
            loan.Member = member;
            loan.BorrowedAt = DateTime.UtcNow;
            return (await _loanRepository.Create(loan)).ToDto();
        }

        public async Task Delete(int id)
        {
            bool deleted = await _loanRepository.Delete(id);
            if (!deleted)
                throw new LoanNotFoundException(id);
        }

        public async Task ReturnBook(int id)
        {
            Loan loan = await _loanRepository.GetByIdWithRelationships(id) ?? throw new LoanNotFoundException(id);
            loan.Return();
            await _loanRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<LoanResponseDto>> GetAll()
        {
            return await _loanRepository.GetAll();
        }

        public async Task<LoanResponseDto> GetById(int id)
        {
            Loan loan = await _loanRepository.GetByIdWithRelationships(id) ?? throw new LoanNotFoundException(id);

            return loan.ToDto();
        }

        public async Task<IEnumerable<LoanResponseDto>> GetMyLoans(int id)
        {
            return await _loanRepository.GetMyLoans(id);
        }
    }
}
