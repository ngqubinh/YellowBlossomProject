'use client';

import {
  Home,
  Inbox,
  Calendar,
  Search,
  Settings,
  User2,
  ChevronUp,
  Plus,
  Projector,
  ChevronDown,
} from "lucide-react";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupAction,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuBadge,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarMenuSub,
  SidebarMenuSubButton,
  SidebarMenuSubItem,
  SidebarSeparator,
} from "./ui/sidebar";
import Link from "next/link";
import Image from "next/image";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "./ui/dropdown-menu";
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "./ui/collapsible";
import { useAuth } from "./providers/AuthProvider";

// Define role-based items
const items = [
  {
    title: "Home",
    url: "/",
    icon: Home,
    roles: ["ADMIN", "ProjectManager", "user"], // Visible to all roles
  },
  {
    title: "Inbox",
    url: "#",
    icon: Inbox,
    roles: ["ADMIN", "ProjectManager", "user"],
  },
  {
    title: "Calendar",
    url: "#",
    icon: Calendar,
    roles: ["ADMIN", "ProjectManager"],
  },
  {
    title: "Search",
    url: "#",
    icon: Search,
    roles: ["ADMIN", "ProjectManager"],
  },
  {
    title: "Settings",
    url: "#",
    icon: Settings,
    roles: ["ADMIN"], // Only visible to ADMIN
  },
];

const AppSidebar = () => {
  const { user } = useAuth(); // Get user from context
  const userRole = user?.role || "user"; // Fallback to "user" if no role

  return (
    <Sidebar collapsible="icon">
      <SidebarHeader className="py-4">
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton asChild>
              <Link href="/">
                <Image src="/logo.svg" alt="logo" width={20} height={20} />
                <span>PMIS</span>
              </Link>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>
      <SidebarSeparator />
      <SidebarContent>
        {/* <SidebarGroup>
          <SidebarGroupLabel>Application</SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu>
              {items
                .filter((item) => item.roles.includes(userRole))
                .map((item) => (
                  <SidebarMenuItem key={item.title}>
                    <SidebarMenuButton asChild>
                      <Link href={item.url}>
                        <item.icon />
                        <span>{item.title}</span>
                      </Link>
                    </SidebarMenuButton>
                    {item.title === "Inbox" && (
                      <SidebarMenuBadge>24</SidebarMenuBadge>
                    )}
                  </SidebarMenuItem>
                ))}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup> */}

        {userRole === "ADMIN" && (
          <SidebarGroup>
            <SidebarGroupLabel>Thống kê)</SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                <SidebarMenuItem>
                  <SidebarMenuButton asChild>
                    <Link href="/dashboard">
                      <Projector />
                      Dashboard
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        )}

        {/* Sản phẩm group - Only for ADMIN */}
        {userRole === "ADMIN" && (
          <SidebarGroup>
            <SidebarGroupLabel>Sản phẩm</SidebarGroupLabel>
            {/* <SidebarGroupAction>
              <Plus /> <span className="sr-only">Thêm mới</span>
            </SidebarGroupAction> */}
            <SidebarGroupContent>
              <SidebarMenu>
                <SidebarMenuItem>
                  <SidebarMenuButton asChild>
                    <Link href="/product">
                      <Projector />
                      Tất cả sản phẩm
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
                {/* <SidebarMenuItem>
                  <SidebarMenuButton asChild>
                    <Link href="/#">
                      <Plus />
                      Thêm mới
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem> */}
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        )}

        {/* Người dùng/Nhóm(Team) group - Only for ADMIN */}
        {userRole === "ADMIN" && (
          <SidebarGroup>
            <SidebarGroupLabel>Người dùng/Nhóm(Team)</SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                <SidebarMenuItem>
                  <SidebarMenuButton asChild>
                    <Link href="/user">
                      <Projector />
                      Xem tất cả người dùng
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
                <SidebarMenuItem>
                  <SidebarMenuButton asChild>
                    <Link href="/team">
                      <Projector />
                      Xem tất cả nhóm(team)
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
               
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        )}

        {userRole === "ProjectManager" && (
          <SidebarGroup>
            <SidebarGroupLabel>Người dùng/Nhóm(Team)</SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                <SidebarMenuItem>
                  <SidebarMenuButton asChild>
                    <Link href="/team">
                      <Projector />
                      Xem tất cả nhóm(team)
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
               
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        )}

        {/* Projects group - For ADMIN and ProjectManager */}
        {(userRole === "ProjectManager") && (
          <SidebarGroup>
            <SidebarGroupLabel>Dự án</SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                <SidebarMenuItem>
                  <SidebarMenuButton asChild>
                    <Link href="/project/related-page">
                      <Projector />
                      Xem tất cả dự án liên quan
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        )}

        {(userRole === "Developer") && (
          <SidebarGroup>
            <SidebarGroupLabel>Công việc của tôi</SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                <SidebarMenuItem>
                  <SidebarMenuButton asChild>
                    <Link href="/task-related">
                      <Projector />
                      Xem tất cả công việc
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        )}

        {(userRole === "QA") && (
          <SidebarGroup>
            <SidebarGroupLabel>Công việc đã hoàn thành</SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                <SidebarMenuItem>
                  <SidebarMenuButton asChild>
                    <Link href="/test">
                      <Projector />
                      Xem tất cả công việc đã hoàn thành
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        )}
        
        {(userRole === "Tester") && (
          <SidebarGroup>
            <SidebarGroupLabel>Công việc cần test</SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                <SidebarMenuItem>
                  <SidebarMenuButton asChild>
                    <Link href="/tester">
                      <Projector />
                      Xem tất cả công việc cần test
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        )}

      </SidebarContent>
      <SidebarFooter>
        <SidebarMenu>
          <SidebarMenuItem>
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <SidebarMenuButton>
                  <User2 /> {user?.email || "?"} <ChevronUp className="ml-auto" />
                </SidebarMenuButton>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end">
                <DropdownMenuItem>Account</DropdownMenuItem>
                <DropdownMenuItem>Setting</DropdownMenuItem>
                <DropdownMenuItem>Sign out</DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarFooter>
    </Sidebar>
  );
};

export default AppSidebar;