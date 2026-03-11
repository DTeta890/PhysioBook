import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { useTranslation } from 'react-i18next'
import { useNavigate } from '@tanstack/react-router'
import { Input } from '@/shared/components/Input'
import { Button } from '@/shared/components/Button'
import { useAuth } from '@/features/auth/hooks/useAuth'
import { authApi } from '@/features/auth/api/auth-api'
import { ApiError } from '@/lib/api-client'

interface LoginFormData {
  email: string
  password: string
  tenantId: string
}

export function LoginPage() {
  const { t, i18n } = useTranslation()
  const navigate = useNavigate()
  const login = useAuth((s) => s.login)
  const storedTenantId = useAuth((s) => s.tenantId)
  const [serverError, setServerError] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormData>({
    defaultValues: {
      tenantId: storedTenantId ?? 'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11', // demo tenant
    },
  })

  const onSubmit = async (data: LoginFormData) => {
    setServerError(null)
    try {
      const response = await authApi.login(data.email, data.password, data.tenantId)
      login(
        { accessToken: response.data.accessToken, refreshToken: response.data.refreshToken },
        response.data.user,
      )
      void navigate({ to: '/calendar' })
    } catch (error) {
      if (error instanceof ApiError) {
        setServerError(error.message)
      } else {
        setServerError(t('common.error'))
      }
    }
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

        {serverError && (
          <div className="rounded-lg bg-red-50 p-3 text-sm text-red-700">
            {serverError}
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <Input
            label={t('auth.email')}
            type="email"
            placeholder="email@example.com"
            autoComplete="email"
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
            autoComplete="current-password"
            error={errors.password?.message}
            {...register('password', {
              required: t('auth.passwordRequired'),
              minLength: { value: 6, message: t('auth.passwordMin') },
            })}
          />

          {/* Hidden tenant ID field for demo — will be replaced by subdomain/slug resolution */}
          <input type="hidden" {...register('tenantId')} />

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
