using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace BinaryStudio.PlatformUI.Media.Effects
    {
    public sealed class GrayscaleEffect : ShaderEffect
        {
        private static PixelShader shader = null;

        public GrayscaleEffect() {
            if (shader == null) {
                shader = new PixelShader { UriSource = new Uri(@"pack://application:,,,/BinaryStudio.PlatformUI;component/GrayscaleEffect.ps") };
                }
            PixelShader = shader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(DesaturationFactorProperty);
            }

        #region P:Input:Brush
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(GrayscaleEffect), 0);
        public Brush Input {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
            }
        #endregion
        #region P:DesaturationFactor:Double
        public static readonly DependencyProperty DesaturationFactorProperty = DependencyProperty.Register("DesaturationFactor", typeof(Double), typeof(GrayscaleEffect), new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0), CoerceDesaturationFactor));
        private static Object CoerceDesaturationFactor(DependencyObject d, Object value) {
            var effect = (GrayscaleEffect)d;
            var factor = (Double)value;
            if (factor < 0.0 || factor > 1.0) {
                return effect.DesaturationFactor;
                }
            return factor;
            }
        public Double DesaturationFactor {
            get { return (Double)GetValue(DesaturationFactorProperty); }
            set { SetValue(DesaturationFactorProperty, value); }
            }
        #endregion
        }
    }