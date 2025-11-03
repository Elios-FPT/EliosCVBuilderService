<!-- e46656ef-61d1-4cc0-82be-f97fca4a0e96 75fa4ade-9949-47ba-9be4-63a6452aad44 -->
# Create Unit Tests for CV Builder Controllers

## Overview

Create comprehensive xUnit test suites for `CvTemplatesController` and `UserCvsController`, following the existing test pattern from `CreateTemplateTest.cs`.

## Test Structure

### CVBuilder.Test Project Setup

- Verify/Create test project with xUnit, Moq, and required dependencies
- Reference CVBuilder.Web, CVBuilder.Contract projects

### CvTemplatesController Tests (5 test classes in `CvTemplate/` folder)

**1. CreateTemplateTest.cs** (Already exists as reference)

- `CreateTemplate_ValidRequest_ReturnsSuccess`
- `CreateTemplate_NullName_ReturnsBadRequest`
- `CreateTemplate_Exception_ReturnsErrorStatus`

**2. GetTemplateByIdTest.cs**

- `GetTemplate_ValidId_ReturnsSuccess`
- `GetTemplate_NonExistentId_ReturnsNotFound`
- `GetTemplate_InvalidId_ReturnsBadRequest`
- `GetTemplate_Exception_ReturnsErrorStatus`

**3. GetAllTemplatesTest.cs**

- `GetTemplates_ValidRequest_ReturnsSuccess`
- `GetTemplates_WithPagination_ReturnsPagedResults`
- `GetTemplates_IncludeDeleted_ReturnsAllTemplates`
- `GetTemplates_InvalidPageNumber_ReturnsBadRequest`
- `GetTemplates_Exception_ReturnsErrorStatus`

**4. UpdateTemplateTest.cs**

- `UpdateTemplate_ValidRequest_ReturnsSuccess`
- `UpdateTemplate_NonExistentId_ReturnsNotFound`
- `UpdateTemplate_NullName_ReturnsBadRequest`
- `UpdateTemplate_Exception_ReturnsErrorStatus`

**5. DeleteTemplateTest.cs**

- `DeleteTemplate_ValidId_ReturnsSuccess`
- `DeleteTemplate_NonExistentId_ReturnsNotFound`
- `DeleteTemplate_Exception_ReturnsErrorStatus`

### UserCvsController Tests (5 test classes in `UserCv/` folder)

**1. CreateUserCvTest.cs**

- `CreateUserCv_ValidRequest_ReturnsSuccess`
- `CreateUserCv_NullTitle_ReturnsBadRequest`
- `CreateUserCv_InvalidTemplateId_ReturnsNotFound`
- `CreateUserCv_Exception_ReturnsErrorStatus`

**2. GetUserCvByIdTest.cs**

- `GetUserCv_ValidId_ReturnsSuccess`
- `GetUserCv_NonExistentId_ReturnsNotFound`
- `GetUserCv_Exception_ReturnsErrorStatus`

**3. GetUserCvsTest.cs**

- `GetUserCvs_ValidRequest_ReturnsSuccess`
- `GetUserCvs_WithPagination_ReturnsPagedResults`
- `GetUserCvs_NoResults_ReturnsEmptyList`
- `GetUserCvs_InvalidPageNumber_ReturnsBadRequest`
- `GetUserCvs_Exception_ReturnsErrorStatus`

**4. UpdateUserCvTest.cs**

- `UpdateUserCv_ValidRequest_ReturnsSuccess`
- `UpdateUserCv_NonExistentId_ReturnsNotFound`
- `UpdateUserCv_NullTitle_ReturnsBadRequest`
- `UpdateUserCv_Exception_ReturnsErrorStatus`

**5. DeleteUserCvTest.cs**

- `DeleteUserCv_ValidId_ReturnsSuccess`
- `DeleteUserCv_NonExistentId_ReturnsNotFound`
- `DeleteUserCv_Exception_ReturnsErrorStatus`

## Test Pattern

Each test class follows:

```csharp
- Mock ISender (MediatR)
- Inject into controller constructor
- Arrange: Create request + expected response
- Setup: Mock _sender.Send() behavior
- Act: Call controller method
- Assert: Verify response + mock invocations
```

## Key Files Referenced

- `CVBuilder/Controllers/CvTemplatesController.cs` - 5 endpoints
- `CVBuilder/Controllers/UserCvsController.cs` - 5 endpoints  
- `CVBuilder.Contract/UseCases/CvTemplate/*` - Commands/Queries/Requests
- `CVBuilder.Contract/UseCases/UserCv/*` - Commands/Queries/Requests
- Test reference: `CreateTemplateTest.cs`

### To-dos

- [ ] Create/verify CVBuilder.Test project with xUnit, Moq dependencies and folder structure
- [ ] Create GetTemplateByIdTest.cs with 4 test methods
- [ ] Create GetAllTemplatesTest.cs with 5 test methods
- [ ] Create UpdateTemplateTest.cs with 4 test methods
- [ ] Create DeleteTemplateTest.cs with 3 test methods
- [ ] Create CreateUserCvTest.cs with 4 test methods
- [ ] Create GetUserCvByIdTest.cs with 3 test methods
- [ ] Create GetUserCvsTest.cs with 5 test methods
- [ ] Create UpdateUserCvTest.cs with 4 test methods
- [ ] Create DeleteUserCvTest.cs with 3 test methods