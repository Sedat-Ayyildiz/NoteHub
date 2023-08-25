using MvcProject.BusinessLayer.Abstract;
using MvcProject.BusinessLayer.Results;
using MvcProject.Common.Helpers;
using MvcProject.DataAccessLayer.EntityFramework;
using MvcProject.Entities;
using MvcProject.Entities.Messages;
using MvcProject.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcProject.BusinessLayer
{
    public class MvcProjectUserManager : ManagerBase<MvcProjectUser>
    {
        public BusinessLayerResult<MvcProjectUser> RegisterUser(RegisterViewModel data)
        {
            MvcProjectUser user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<MvcProjectUser> res = new BusinessLayerResult<MvcProjectUser>();

            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı Adı Kayıtlı !");
                }
                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotInserted, "E-Posta Adresi Kayıtlı !");
                }
            }
            else
            {
                int dbResult = base.Insert(new MvcProjectUser()
                {
                    Username = data.Username,
                    Email = data.Email,
                    ProfileImageFilename = "user_boy.png",
                    Password = data.Password,
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = false,
                    IsAdmin = false
                });
                if (dbResult > 0)
                {
                    res.Result = Find(x => x.Email == data.Email && x.Username == data.Username);

                    string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                    string activateUri = $"{siteUri}/Home/UserActivate/{res.Result.ActivateGuid}";
                    string body = $"Merhaba {res.Result.Username};<br><br>NotHub'a hoş geldiniz! Kayıt işleminizi tamamlamanız için bu e-postayı alıyorsunuz. NotHub, sizin için not tutmayı kolay ve organize hale getiren ve istediğiniz konular hakkında fikirlerinizi beyan etmenizi sağlayan bir platformdur. İşte hesabınızı etkinleştirmek için gereken adımlar:<br><br>" +
                        $"1-)NotHub hesabınızı etkinleştirmek için aşağıdaki bağlantıya tıklayın: <a href='{activateUri}' target='_blank'>Hesabımı aktifleştir</a><br>" +
                        $"2-)Linke tıkladıktan sonra, açılan sayfada bir süre bekledikten sonra hesabınız onaylanır ve böylece not ekleme, beğenme vb. işlemleri artık yapabilirsiniz.<br><br>" +
                        $"Not: Kayıt işleminizi tamamlamadan önce, lütfen NoteHub kullanıcı sözleşmesini ve gizlilik politikasını okuduğunuzdan emin olun. Bu belgeler, NotHub kullanımınızı yönlendirecek önemli bilgiler içermektedir.<br><br>" +
                        $"Eğer bu e-postayı talep etmediyseniz, lütfen dikkate almayın ve hesabınızın güvende olduğundan emin olun.<br><br>" +
                        $"NotHub'a katıldığınız için teşekkür ederiz! Sizlere en iyi not tutma deneyimini sunmak ve istediğiniz konular hakkında fikirlerinizi beyan etmeniz için buradayız. Herhangi bir sorunuz veya sorununuz varsa, lütfen bize ulaşmaktan çekinmeyin.<br><br>" +
                        $"Mail Adresi : NoteHub@outlook.com.tr<br><br>" +
                        $"Saygılarımızla,<br>NotHub Ekibi";
                    MailHelper.SendMail(body, res.Result.Email, "NoteHub Hesap Aktifleştirme");
                }
            }
            return res;
        }

        public BusinessLayerResult<MvcProjectUser> LoginUser(LoginViewModel data)
        {
            BusinessLayerResult<MvcProjectUser> res = new BusinessLayerResult<MvcProjectUser>();
            res.Result = Find(x => x.Username == data.Username && x.Password == data.Password);

            if (res.Result != null)
            {
                if (!res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserIsNotActive, "Kullanıcı aktifleştirilmemiştir.");
                    res.AddError(ErrorMessageCode.CheckYourEmail, "Lütfen E-Posta Adresinizi kontrol ediniz.");
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UsernameOrPassWrong, "Kullanıcı Adı ya da Şifre uyuşmuyor !");
            }
            return res;
        }

        public BusinessLayerResult<MvcProjectUser> ActivateUser(Guid activateId)
        {
            BusinessLayerResult<MvcProjectUser> res = new BusinessLayerResult<MvcProjectUser>();
            res.Result = Find(x => x.ActivateGuid == activateId);

            if (res.Result != null)
            {
                if (res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserAlreadyActive, "Kullanıcı zaten aktif edilmiştir.");
                    return res;
                }
                res.Result.IsActive = true;
                Update(res.Result);
            }
            else
            {
                res.AddError(ErrorMessageCode.ActivateIdDoesNotExists, "Aktifleştirilecek kullanıcı bulunamadı.");
            }
            return res;
        }

        public BusinessLayerResult<MvcProjectUser> GetUserById(int id)
        {
            BusinessLayerResult<MvcProjectUser> res = new BusinessLayerResult<MvcProjectUser>();
            res.Result = Find(x => x.Id == id);
            if (res.Result == null)
            {
                res.AddError(ErrorMessageCode.UserNotFound, "Kullanıcı bulunamadı.");
            }
            return res;
        }

        public BusinessLayerResult<MvcProjectUser> UpdateProfile(MvcProjectUser data)
        {
            MvcProjectUser db_user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<MvcProjectUser> res = new BusinessLayerResult<MvcProjectUser>();

            if (db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı Adı Kayıtlı !");
                }
                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotInserted, "E-Posta Adresi Kayıtlı !");
                }
                return res;
            }
            res.Result = Find(x => x.Id == data.Id);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;
            if (string.IsNullOrEmpty(data.ProfileImageFilename) == false)
            {
                res.Result.ProfileImageFilename = data.ProfileImageFilename;
            }
            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.ProfileCouldNotUpdated, "Profil Güncellenemedi.");
            }
            return res;
        }

        public BusinessLayerResult<MvcProjectUser> RemoveUserById(int id)
        {
            BusinessLayerResult<MvcProjectUser> res = new BusinessLayerResult<MvcProjectUser>();
            MvcProjectUser user = Find(x => x.Id == id);

            if (user != null)
            {
                if (Delete(user) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotRemove, "Kullanıcı Silinemedi !");
                    return res;
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UserCouldNotFind, "Kullanıcı Bulunamadı !");
            }
            return res;
        }
        //--------------------------------------------------------------------------------------------------------------
        public new BusinessLayerResult<MvcProjectUser> Insert(MvcProjectUser data)
        {
            MvcProjectUser user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<MvcProjectUser> res = new BusinessLayerResult<MvcProjectUser>();

            res.Result = data;
            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı Adı Kayıtlı !");
                }
                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotInserted, "E-Posta Adresi Kayıtlı !");
                }
            }
            else
            {
                res.Result.ProfileImageFilename = "user_boy.png";
                res.Result.ActivateGuid = Guid.NewGuid();

                if (base.Insert(res.Result) == 0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotInserted, "Kullanıcı Eklenemedi !");
                }
            }
            return res;
        }

        public new BusinessLayerResult<MvcProjectUser> Update(MvcProjectUser data)
        {
            MvcProjectUser db_user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<MvcProjectUser> res = new BusinessLayerResult<MvcProjectUser>();
            res.Result= data;

            if (db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı Adı Kayıtlı !");
                }
                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotInserted, "E-Posta Adresi Kayıtlı !");
                }
                return res;
            }

            res.Result = Find(x => x.Id == data.Id);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;
            res.Result.IsActive= data.IsActive;
            res.Result.IsAdmin= data.IsAdmin;

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.UserCouldNotUpdated, "Kullanıcı Güncellenemedi.");
            }
            return res;
        }
    }
}
