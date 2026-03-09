import { useForm } from 'react-hook-form'
import { z } from 'zod'
import { useTranslation } from 'react-i18next'
import { Input } from '@/shared/components/Input'
import { Button } from '@/shared/components/Button'

const loginSchema = z.object({
  email: z.string().min(1, 'Email is required').email('Invalid email'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
})

type LoginFormData = z.infer<typeof loginSchema>

export function LoginPage() {
  const { t, i18n } = useTranslation()
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormData>()

  const onSubmit = async (_data: LoginFormData) => {
    // TODO: Call auth API and useAuth.login()
  }

  const toggleLanguage = () => {
    const next = i18n.language === 'sq' ? 'en' : 'sq'
    void i18n.changeLanguage(next)
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-primary-50">
      <div className="w-full max-w-md space-y-8 rounded-xl bg-white p-8 shadow-lg">
        <div className="text-center">
          <h1 className="text-3xl font-bold text-primary-700">PhysioBook</h1>
          <p className="mt-2 text-sm text-gray-600">{t('auth.loginSubtitle')}</p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <Input
            label={t('auth.email')}
            type="email"
            placeholder="email@example.com"
            error={errors.email?.message}
            {...register('email', {
              required: t('auth.emailRequired'),
              pattern: { value: /^\S+@\S+$/i, message: t('auth.emailInvalid') },
            })}
          />
          <Input
            label={t('auth.password')}
            type="password"
            placeholder="••••••••"
            error={errors.password?.message}
            {...register('password', {
              required: t('auth.passwordRequired'),
              minLength: { value: 6, message: t('auth.passwordMin') },
            })}
          />
          <Button type="submit" className="w-full" disabled={isSubmitting}>
            {isSubmitting ? t('common.loading') : t('auth.login')}
          </Button>
        </form>

        <div className="text-center">
          <button
            type="button"
            onClick={toggleLanguage}
            className="text-sm text-primary-600 hover:text-primary-700"
          >
            {i18n.language === 'sq' ? 'English' : 'Shqip'}
          </button>
        </div>
      </div>
    </div>
  )
}
