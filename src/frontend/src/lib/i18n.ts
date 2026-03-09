import i18n from 'i18next'
import { initReactI18next } from 'react-i18next'

import sq from '@/lib/i18n/locales/sq.json'
import en from '@/lib/i18n/locales/en.json'

void i18n.use(initReactI18next).init({
  resources: {
    sq: { translation: sq },
    en: { translation: en },
  },
  lng: 'sq',
  fallbackLng: 'en',
  interpolation: {
    escapeValue: false,
  },
})

export default i18n
