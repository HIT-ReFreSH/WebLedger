# Contributing to WebLedger / 为 WebLedger 做贡献

Thank you for your interest in contributing to WebLedger! We welcome contributions from the community.

感谢你对为 WebLedger 做贡献的兴趣！我们欢迎来自社区的贡献。

## 📋 How to Contribute / 如何贡献

### 1. Find an Issue / 找到一个 Issue

- Browse the [open issues](https://github.com/HIT-ReFreSH/WebLedger/issues)
- Look for issues labeled `good first issue` if you're new to the project
- Check the [ISSUES_DRAFT.md](./docs/ISSUES_DRAFT.md) for planned issues

### 2. Claim the Issue / 领取 Issue

**IMPORTANT / 重要:**
Before starting work, comment on the issue to let maintainers know you're working on it. This helps avoid duplicate work.

在开始工作之前，请在 issue 上评论，让维护者知道你正在处理它。这有助于避免重复工作。

Example / 示例:
```
I'd like to work on this issue. I'll submit a PR by [date].
我想处理这个 issue。我将在 [日期] 前提交 PR。
```

### 3. Fork and Clone / Fork 和克隆

```bash
# Fork the repository on GitHub
# 在 GitHub 上 Fork 仓库

# Clone your fork
git clone https://github.com/YOUR_USERNAME/WebLedger.git
cd WebLedger

# Add upstream remote
git remote add upstream https://github.com/HIT-ReFreSH/WebLedger.git
```

### 4. Create a Branch / 创建分支

```bash
git checkout -b feature/issue-number-description
# Example: git checkout -b feature/18-react-dashboard
```

### 5. Make Your Changes / 进行更改

Follow the coding standards and requirements specified in the issue.

遵循 issue 中指定的编码标准和要求。

#### For Backend Development / 后端开发

- Use .NET 8
- Follow C# coding conventions
- Add XML documentation comments
- Write unit tests for new functionality
- Ensure all tests pass

#### For Frontend Development / 前端开发

**⚠️ Mandatory Requirements / 强制要求:**

- ✅ Use **Vite** (NOT Create React App or Vue CLI)
- ✅ Use **TypeScript** (NO plain JavaScript)
- ✅ Use **yarn with PnP** or **pnpm** (NOT npm)
- ✅ Follow modern React/Vue best practices
- ✅ Write clean, well-commented code
- ✅ Ensure responsive design

### 6. Test Your Changes / 测试你的更改

```bash
# Backend
cd web
dotnet test

# Frontend
cd your-frontend-project
pnpm test  # or: yarn test
```

### 7. Take Screenshots (Frontend Only) / 拍摄截图（仅前端）

**REQUIRED for all frontend PRs / 所有前端 PR 都需要:**

Take screenshots showing:
- Desktop view
- Mobile view (phone)
- Tablet view (if applicable)
- Dark mode (if implemented)
- Loading states
- Error states
- Different data scenarios

截图应显示：
- 桌面视图
- 移动视图（手机）
- 平板视图（如适用）
- 深色模式（如已实现）
- 加载状态
- 错误状态
- 不同的数据场景

### 8. Commit Your Changes / 提交你的更改

```bash
git add .
git commit -m "feat: add feature description (#issue-number)"

# Follow conventional commits format:
# feat: new feature
# fix: bug fix
# docs: documentation changes
# test: adding tests
# refactor: code refactoring
# style: formatting, missing semicolons, etc.
# chore: maintenance tasks
```

### 9. Push and Create PR / 推送并创建 PR

```bash
git push origin feature/issue-number-description
```

Then go to GitHub and create a Pull Request. The PR template will be automatically loaded.

然后转到 GitHub 并创建 Pull Request。PR 模板将自动加载。

## 📝 Pull Request Requirements / PR 要求

### All PRs / 所有 PR

- [ ] Clear description of what was done and why
- [ ] Reference the related issue number (e.g., "Closes #18")
- [ ] Code follows project coding standards
- [ ] All tests pass
- [ ] No merge conflicts
- [ ] Documentation updated (if applicable)

### Frontend PRs / 前端 PR

**⚠️ Your PR will NOT be reviewed without these / 没有这些你的 PR 将不会被审查:**

- [ ] **Screenshots included** (mandatory)
- [ ] Uses Vite (not CRA or Vue CLI)
- [ ] Uses TypeScript (no .js files)
- [ ] Uses yarn PnP or pnpm (not npm)
- [ ] README with setup instructions included
- [ ] Code is responsive
- [ ] No console errors

## 🎨 Coding Standards / 编码标准

### Backend / 后端

```csharp
// Use XML documentation
/// <summary>
/// Description of the method
/// </summary>
/// <param name="param">Parameter description</param>
/// <returns>Return value description</returns>
public async Task<Result> MethodName(Type param)
{
    // Use meaningful variable names
    // Follow C# naming conventions
    // Add comments for complex logic
}
```

### Frontend / 前端

**React:**
```typescript
// Use functional components with TypeScript
interface Props {
  title: string;
  onSubmit: (data: FormData) => void;
}

export const MyComponent: React.FC<Props> = ({ title, onSubmit }) => {
  // Use hooks appropriately
  // Add proper types
  // Write clean, readable code

  return (
    <div>
      {/* JSX here */}
    </div>
  );
};
```

**Vue:**
```vue
<script setup lang="ts">
// Use Composition API with TypeScript
interface Props {
  title: string;
  onSubmit: (data: FormData) => void;
}

const props = defineProps<Props>();
// Use composables appropriately
// Add proper types
</script>

<template>
  <!-- Template here -->
</template>
```

## ❌ Common Mistakes to Avoid / 常见错误

### For Frontend PRs / 前端 PR

1. ❌ Using Create React App instead of Vite
2. ❌ Using JavaScript instead of TypeScript
3. ❌ Using npm instead of yarn PnP/pnpm
4. ❌ Not including screenshots
5. ❌ Not making it responsive
6. ❌ Not testing on different screen sizes
7. ❌ Not commenting on issue before starting

### For All PRs / 所有 PR

1. ❌ Not referencing the issue number
2. ❌ Making changes to unrelated files
3. ❌ Not testing locally before submitting
4. ❌ Not following the coding standards
5. ❌ Submitting PRs with merge conflicts

## 🔍 Review Process / 审查流程

1. Maintainers will review your PR within a few days
2. You may be asked to make changes
3. Once approved, your PR will be merged
4. Your contribution will be credited

维护者将在几天内审查你的 PR。你可能会被要求进行更改。一旦批准，你的 PR 将被合并。你的贡献将被记入。

## 💬 Getting Help / 获取帮助

- **Documentation**: Check the `docs/` folder
- **Questions**: Ask in the issue comments
- **API Documentation**: Visit `/swagger` when running the backend
- **Examples**: Look at existing code

## 📚 Additional Resources / 额外资源

- [Getting Started Guide](./docs/getting-started.md)
- [Frontend Integration Guide](./docs/frontend-integration.md)
- [Issues Draft](./docs/ISSUES_DRAFT.md)
- [Vite Documentation](https://vitejs.dev/)
- [TypeScript Documentation](https://www.typescriptlang.org/)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)

## 🙏 Thank You / 感谢

Thank you for contributing to WebLedger! Every contribution, no matter how small, helps make this project better.

感谢你为 WebLedger 做贡献！无论多小的贡献，都有助于让这个项目变得更好。
